using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Server;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Handshake;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Handshake;
using Sadie.Networking.Writers.Moderation;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Clothing;
using Sadie.Networking.Writers.Players.Effects;
using Sadie.Networking.Writers.Players.Navigator;
using Sadie.Networking.Writers.Players.Other;
using Sadie.Networking.Writers.Players.Permission;
using Sadie.Networking.Writers.Players.Rooms;
using Sadie.Options.Options;
using Sadie.Shared;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Events.Handlers.Handshake;

public class SecureLoginEventHandler(
    SecureLoginEventParser eventParser,
    ILogger<SecureLoginEventHandler> logger,
    IOptions<EncryptionOptions> encryptionOptions,
    PlayerRepository playerRepository,
    ServerPlayerConstants constants,
    INetworkClientRepository networkClientRepository,
    ServerSettings serverSettings)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.SecureLogin;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (encryptionOptions.Value.Enabled && !client.EncryptionEnabled)
        {
            logger.LogWarning("Encryption is enabled and TLS Handshake isn't finished.");
            await DisconnectAsync(client.Channel.Id);
            return;
        }

        if (!ValidateSso(eventParser.Token))
        {
            logger.LogWarning("Rejected an insecure sso token");
            await DisconnectAsync(client.Channel.Id);
            return;
        }

        var token = await playerRepository.GetSsoTokenAsync(eventParser.Token, eventParser.Delay);

        if (token == null)
        {
            logger.LogWarning("Failed to find token record for provided sso.");
            await DisconnectAsync(client.Channel.Id);
            return;
        }

        var player = await playerRepository.CreatePlayerInstanceWithIdAsync(client, token.PlayerId);

        if (player == null)
        {
            logger.LogError("Failed to resolve player record.");
            await DisconnectAsync(client.Channel.Id);
            return;
        }

        var playerId = player.Id;

        client.Player = player;

        if (!playerRepository.TryAddPlayer(player))
        {
            logger.LogError($"Player {playerId} could not be registered");
            await DisconnectAsync(client.Channel.Id);
            return;
        }

        logger.LogInformation($"Player '{player.Username}' has logged in");
        await playerRepository.UpdateOnlineStatusAsync(playerId, true);

        player.Data.LastOnline = DateTime.Now;
        player.Authenticated = true;

        await SendExtraPacketsAsync(client, player);
        await SendSubscriptionPacketsAsync(player);
        await SendFriendUpdatesAsync(player);
        await SendWelcomeMessageAsync(player);
    }

    private async Task SendSubscriptionPacketsAsync(PlayerLogic player)
    {
        foreach (var playerSub in player.Subscriptions)
        {
            var tillExpire = playerSub.ExpiresAt - playerSub.CreatedAt;
            var daysLeft = (int)tillExpire.TotalDays;
            var minutesLeft = (int)tillExpire.TotalMinutes;
            var minutesSinceMod = (int)(DateTime.Now - player.State.LastSubscriptionModification).TotalMinutes;

            await player.NetworkObject.WriteToStreamAsync(new PlayerSubscriptionWriter
            {
                Name = playerSub.Subscription.Name,
                DaysLeft = daysLeft,
                MemberPeriods = 0,
                PeriodsSubscribedAhead = 0,
                ResponseType = 1,
                HasEverBeenMember = true,
                IsVip = true,
                PastClubDays = 0,
                PastVipDays = 0,
                MinutesTillExpire = minutesLeft,
                MinutesSinceModified = minutesSinceMod
            });

            player.State.LastSubscriptionModification = DateTime.Now;
        }
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

    private async Task SendExtraPacketsAsync(INetworkObject networkObject, PlayerLogic player)
    {
        var playerData = player.Data;
        var playerSubscriptions = player.Subscriptions;

        await networkObject.WriteToStreamAsync(new SecureLoginWriter());

        await networkObject.WriteToStreamAsync(new NoobnessLevelWriter
        {
            Level = 1
        });

        await networkObject.WriteToStreamAsync(new PlayerHomeRoomWriter
        {
            HomeRoom = playerData.HomeRoomId,
            RoomIdToEnter = playerData.HomeRoomId
        });

        await networkObject.WriteToStreamAsync(new PlayerEffectListWriter
        {
            Effects = []
        });

        await networkObject.WriteToStreamAsync(new PlayerClothingListWriter());

        await networkObject.WriteToStreamAsync(new PlayerPermissionsWriter
        {
            Club = playerSubscriptions.Any(x => x.Subscription.Name == "HABBO_CLUB") ? 2 : 0,
            Rank = player.Roles.Count != 0 ? player.Roles.Max(x => x.Id) : 0,
            Ambassador = true
        });

        var navigatorSettingsWriter = new PlayerNavigatorSettingsWriter
        {
            NavigatorSettings = player.NavigatorSettings!
        };

        var statusWriter = new PlayerStatusWriter
        {
            IsOpen = true,
            IsShuttingDown = false,
            IsAuthentic = true
        };

        await networkObject.WriteToStreamAsync(navigatorSettingsWriter);
        await networkObject.WriteToStreamAsync(statusWriter);

        await networkObject.WriteToStreamAsync(new PlayerNotificationSettingsWriter
        {
            ShowNotifications = player.GameSettings.ShowNotifications
        });

        await networkObject.WriteToStreamAsync(new PlayerAchievementScoreWriter
        {
            AchievementScore = playerData.AchievementScore
        });

        if (player.HasPermission("moderation_tools"))
        {
            await networkObject.WriteToStreamAsync(new ModerationToolsWriter
            {
                Unknown1 = 0,
                Unknown2 = 0,
                Unknown3 = 0,
                Unknown4 = true,
                Unknown5 = true,
                Unknown6 = true,
                Unknown7 = true,
                Unknown8 = true,
                Unknown9 = true,
                Unknown10 = true,
                Unknown11 = 0
            });
        }
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

    private bool ValidateSso(string sso) => !string.IsNullOrEmpty(sso) && sso.Length >= constants.MinSsoLength;

    private async Task DisconnectAsync(IChannelId channelId)
    {
        if (!await networkClientRepository.TryRemoveAsync(channelId))
        {
            logger.LogError("Failed to remove network client");
        }
    }
}