using System.Diagnostics;
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
using Sadie.Networking.Client;
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
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (encryptionOptions.Value.Enabled && !client.EncryptionEnabled)
        {
            logger.LogWarning("Encryption is enabled and TLS Handshake isn't finished.");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }

        if (string.IsNullOrEmpty(Token) || !ValidateSso(Token))
        {
            logger.LogWarning("Rejected an insecure sso token");
            await DisconnectNetworkClientAsync(client.Channel.Id);
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
            await DisconnectNetworkClientAsync(client.Channel.Id);
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
            .Include(x => x.Relationships).ThenInclude(x => x.TargetPlayer)
            .Include(x => x.NavigatorSettings)
            .Include(x => x.GameSettings)
            .Include(x => x.Badges)
            .Include(x => x.FurnitureItems).ThenInclude(x => x.FurnitureItem)
            .Include(x => x.FurnitureItems).ThenInclude(x => x.PlacementData)
            .Include(x => x.WardrobeItems)
            .Include(x => x.Roles).ThenInclude(x => x.Permissions)
            .Include(x => x.Subscriptions).ThenInclude(x => x.Subscription)
            .Include(x => x.Respects)
            .Include(x => x.SavedSearches)
            .Include(x => x.OutgoingFriendships).ThenInclude(x => x.TargetPlayer).ThenInclude(x => x.AvatarData)
            .Include(x => x.IncomingFriendships).ThenInclude(x => x.OriginPlayer).ThenInclude(x => x.AvatarData)
            .Include(x => x.MessagesSent)
            .Include(x => x.MessagesReceived)
            .Include(x => x.Rooms).ThenInclude(x => x.Settings)
            .Include(x => x.Bots)
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        if (player == null)
        {
            logger.LogError("Failed to resolve player record.");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }
        
        var playerLogic = mapper.Map<PlayerLogic>(player);

        playerLogic.NetworkObject = client;
        playerLogic.Channel = client.Channel;

        var playerId = player.Id;
        var alreadyOnline = playerRepository.GetPlayerLogicById(playerId) != null;

        client.Player = playerLogic;

        if (alreadyOnline)
        {
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }

        if (!playerRepository.TryAddPlayer(playerLogic))
        {
            logger.LogError($"Player {playerLogic.Username} could not be registered");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }

        logger.LogInformation($"Player '{playerLogic.Username}' has logged in");

        if (!alreadyOnline)
        {
            player.Data.IsOnline = true;
            dbContext.Entry(player.Data).Property(x => x.IsOnline).IsModified = true;
            await dbContext.SaveChangesAsync();
        }

        playerLogic.Data.LastOnline = DateTime.Now;
        playerLogic.Authenticated = true;

        await NetworkPacketEventHelpers.SendLoginPacketsToPlayerAsync(client, playerLogic);
        await NetworkPacketEventHelpers.SendPlayerSubscriptionPacketsAsync(playerLogic);
        
        await PlayerHelpersToClean.SendPlayerFriendListUpdate(playerLogic, playerRepository);
        
        await PlayerHelpersToClean.UpdatePlayerStatusForFriendsAsync(
            player, 
            player.GetMergedFriendships(), 
            true, 
            false, 
            playerRepository);
        
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

        await player.SendAlertAsync(formattedMessage);
    }

    private bool ValidateSso(string sso) => sso.Length >= constants.MinSsoLength;

    private async Task DisconnectNetworkClientAsync(IChannelId channelId)
    {
        if (!await networkClientRepository.TryRemoveAsync(channelId))
        {
            logger.LogError("Failed to remove network client");
        }
    }
}