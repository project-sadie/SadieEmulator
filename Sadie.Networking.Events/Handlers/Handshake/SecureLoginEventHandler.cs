using AutoMapper;
using DotNetty.Transport.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Server;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Options.Options;
using Sadie.Shared;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerIds.SecureLogin)]
public class SecureLoginEventHandler(
    ILogger<SecureLoginEventHandler> logger,
    IOptions<EncryptionOptions> encryptionOptions,
    PlayerRepository playerRepository,
    ServerPlayerConstants constants,
    INetworkClientRepository networkClientRepository,
    ServerSettings serverSettings,
    SadieContext dbContext,
    IMapper mapper)
    : INetworkPacketEventHandler
{
    public string? Token { get; set; }
    public int DelayMs { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (encryptionOptions.Value.Enabled && !client.EncryptionEnabled)
        {
            logger.LogWarning("Encryption is enabled and TLS Handshake isn't finished.");
            await DisconnectAsync(client.Channel.Id);
            return;
        }

        if (string.IsNullOrEmpty(Token) || !ValidateSso(Token))
        {
            logger.LogWarning("Rejected an insecure sso token");
            await DisconnectAsync(client.Channel.Id);
            return;
        }

        var expires = DateTime
            .Now
            .Subtract(TimeSpan.FromMilliseconds(DelayMs));

        var tokenRecord = await dbContext
            .PlayerSsoToken
            .FirstOrDefaultAsync(x =>
                x.Token == Token &&
                x.ExpiresAt > expires &&
                x.UsedAt == null);

        if (tokenRecord == null)
        {
            logger.LogWarning("Failed to find token record for provided sso.");
            await DisconnectAsync(client.Channel.Id);
            return;
        }
        
        tokenRecord.UsedAt = DateTime.Now;

        dbContext.Entry(tokenRecord).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();

        var player = await dbContext
            .Set<Player>()
            .Where(x => x.Id == tokenRecord.PlayerId)
            .Include(x => x.Data)
            .Include(x => x.AvatarData)
            .Include(x => x.Tags)
            .Include(x => x.RoomLikes)
            .Include(x => x.Relationships)
            .Include(x => x.NavigatorSettings)
            .Include(x => x.GameSettings)
            .Include(x => x.Badges)
            .Include(x => x.FurnitureItems).ThenInclude(x => x.FurnitureItem)
            .Include(x => x.WardrobeItems)
            .Include(x => x.Roles).ThenInclude(x => x.Permissions)
            .Include(x => x.Subscriptions).ThenInclude(x => x.Subscription)
            .Include(x => x.Respects)
            .Include(x => x.SavedSearches)
            .Include(x => x.OutgoingFriendships)
            .Include(x => x.OutgoingFriendships)
            .Include(x => x.IncomingFriendships)
            .Include(x => x.IncomingFriendships)
            .Include(x => x.MessagesSent)
            .Include(x => x.MessagesReceived)
            .Include(x => x.Rooms).ThenInclude(x => x.Settings)
            .Include(x => x.Groups)
            .Include(x => x.Bots)
            .FirstOrDefaultAsync();

        if (player == null)
        {
            logger.LogError("Failed to resolve player record.");
            await DisconnectAsync(client.Channel.Id);
            return;
        }
        
        var playerLogic = mapper.Map<PlayerLogic>(player, opt => 
            opt.AfterMap((src, dest) => dest.NetworkObject = client));

        var playerId = player.Id;

        client.Player = playerLogic;

        if (!playerRepository.TryAddPlayer(playerLogic))
        {
            logger.LogError($"Player {playerId} could not be registered");
            await DisconnectAsync(client.Channel.Id);
            return;
        }

        logger.LogInformation($"Player '{playerLogic.Username}' has logged in");
        await playerRepository.UpdateOnlineStatusAsync(playerId, true);

        playerLogic.Data.LastOnline = DateTime.Now;
        playerLogic.Authenticated = true;

        await NetworkPacketEventHelpers.SendLoginPacketsToPlayerAsync(client, playerLogic);
        await NetworkPacketEventHelpers.SendPlayerSubscriptionPacketsAsync(playerLogic);
        
        await SendFriendUpdatesAsync(playerLogic);
        await SendWelcomeMessageAsync(playerLogic);
    }

    private async Task SendWelcomeMessageAsync(PlayerLogic player)
    {
        if (string.IsNullOrEmpty(serverSettings.PlayerWelcomeMessage))
        {
            return;
        }

        var formattedMessage = serverSettings.PlayerWelcomeMessage
            .Replace("[username]", player.Username)
            .Replace("[version]", GlobalState.Version.ToString());

        var alertWriter = new PlayerAlertWriter
        {
            Message = formattedMessage
        };

        await player.NetworkObject.WriteToStreamAsync(alertWriter);
    }

    private async Task SendFriendUpdatesAsync(PlayerLogic player)
    {
        var allFriends = player.GetMergedFriendships();

        await playerRepository.UpdateMessengerStatusForFriends(player.Id,
            allFriends, true, player.CurrentRoomId != 0);

        foreach (var friend in allFriends)
        {
            var friendPlayer = playerRepository.GetPlayerLogicById(friend.TargetPlayerId);
            var isOnline = friendPlayer != null;
            var isInRoom = isOnline && friendPlayer!.CurrentRoomId != 0;

            var relationship = isOnline
                ? friendPlayer!
                    .Relationships
                    .FirstOrDefault(x => x.TargetPlayerId == friend.OriginPlayerId || x.TargetPlayerId == friend.TargetPlayerId) : null;

            await player.NetworkObject.WriteToStreamAsync(new PlayerUpdateFriendWriter
            {
                Unknown1 = 0,
                Unknown2 = 1,
                Unknown3 = 0,
                Friendship = friend,
                IsOnline = isOnline,
                CanFollow = isInRoom,
                CategoryId = 0,
                RealName = "",
                LastAccess = "",
                PersistedMessageUser = false,
                VipMember = false,
                PocketUser = false,
                RelationshipType = (int)(relationship?.TypeId ?? PlayerRelationshipType.None)
            });
        }
    }

    private bool ValidateSso(string sso) => sso.Length >= constants.MinSsoLength;

    private async Task DisconnectAsync(IChannelId channelId)
    {
        if (!await networkClientRepository.TryRemoveAsync(channelId))
        {
            logger.LogError("Failed to remove network client");
        }
    }
}