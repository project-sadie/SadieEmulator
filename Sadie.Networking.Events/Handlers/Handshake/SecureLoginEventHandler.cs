using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Players.Effects;
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
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Events.Handlers.Handshake;

public class SecureLoginEventHandler(
    SecureLoginEventParser eventParser,
    ILogger<SecureLoginEventHandler> logger,
    SadieContext dbContext,
    PlayerRepository playerRepository,
    PlayerConstants constants,
    INetworkClientRepository networkClientRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.SecureLogin;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!ValidateSso(eventParser.Token)) 
        {
            logger.LogWarning("Rejected an insecure sso token");
            await DisconnectAsync(client.Guid);
            return;
        }
            
        var player = await playerRepository.TryGetPlayerBySsoAsync(client, eventParser.Token);

        if (player == null)
        {
            logger.LogWarning("Failed to resolve player from their provided sso");
            
            if (!await networkClientRepository.TryRemoveAsync(client.Guid))
            {
                logger.LogError("Failed to remove network client");
            }
            return;
        }

        var playerData = player.Data;
        var playerId = player.Id;
            
        client.Player = player;
        
        if (!playerRepository.TryAddPlayer(player))
        {
            logger.LogError($"Player {playerId} could not be registered");
            await DisconnectAsync(client.Guid);
            return;
        }
            
        logger.LogInformation($"Player '{player.Username}' has logged in");
        await playerRepository.UpdateOnlineStatusAsync(playerId, true);

        player.Data.LastOnline = DateTime.Now;
        player.Authenticated = true;

        await SendExtraPacketsAsync(client, player);
    }

    private async Task SendExtraPacketsAsync(INetworkObject networkObject, PlayerLogic player)
    {
        var playerData = player.Data;
        var playerSubscriptions = player.Subscriptions;
        
        await networkObject.WriteToStreamAsync(new SecureLoginWriter().GetAllBytes());
        await networkObject.WriteToStreamAsync(new NoobnessLevelWriter(1).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerHomeRoomWriter(playerData.HomeRoomId, playerData.HomeRoomId).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerEffectListWriter(new List<PlayerEffect>()).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerClothingListWriter().GetAllBytes());
        
        await networkObject.WriteToStreamAsync(new PlayerPermissionsWriter(
            playerSubscriptions.Any(x => x.Subscription.Name == "HABBO_CLUB") ? 2 : 0,
            2,
            true).GetAllBytes());
        
        await networkObject.WriteToStreamAsync(new PlayerStatusWriter(true, false, true).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerNavigatorSettingsWriter(player.NavigatorSettings).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerNotificationSettingsWriter(player.GameSettings.ShowNotifications).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerAchievementScoreWriter(playerData.AchievementScore).GetAllBytes());

        foreach (var playerSub in playerSubscriptions)
        {
            var tillExpire = playerSub.ExpiresAt - playerSub.CreatedAt;
            var daysLeft = (int) tillExpire.TotalDays;
            var minutesLeft = (int) tillExpire.TotalMinutes;
            var minutesSinceMod = (int)(DateTime.Now - player.State.LastSubscriptionModification).TotalMinutes;
            
            await networkObject.WriteToStreamAsync(new PlayerSubscriptionWriter(
                playerSub.Subscription.Name,
                daysLeft,
                0, 
                0, 
                1, 
                true, 
                true, 
                0, 
                0, 
                minutesLeft,
                minutesSinceMod).GetAllBytes());
            
            player.State.LastSubscriptionModification = DateTime.Now;
        }

        if (player.HasPermission("moderation_tools"))
        {
            await networkObject.WriteToStreamAsync(new ModerationToolsWriter().GetAllBytes());
        }

        var allFriends = player.GetMergedFriendships();
        
        await playerRepository.UpdateMessengerStatusForFriends(player.Id,
            allFriends, true, player.CurrentRoomId != 0);

        foreach (var friend in allFriends)
        {
            var isOnline = playerRepository.TryGetPlayerById(friend.TargetPlayerId, out var friendPlayer) && friendPlayer != null;
            var isInRoom = isOnline && friendPlayer!.CurrentRoomId != 0;
                    
            var relationship = isOnline
                ? friendPlayer!
                    .Relationships
                    .FirstOrDefault(x => x.TargetPlayerId == friend.OriginPlayerId || x.TargetPlayerId == friend.TargetPlayerId) : null;

            await networkObject.WriteToStreamAsync(new PlayerUpdateFriendWriter(0, 
                1, 
                0,
                friend, 
                isOnline, 
                isInRoom, 
                0, 
                "", 
                "", 
                false, 
                false, 
                false,
                relationship?.TypeId ?? PlayerRelationshipType.None).GetAllBytes());   
        }
    }

    private bool ValidateSso(string sso) => !string.IsNullOrEmpty(sso) && sso.Length >= constants.MinSsoLength;

    private async Task DisconnectAsync(Guid guid)
    {
        if (!await networkClientRepository.TryRemoveAsync(guid))
        {
            logger.LogError("Failed to remove network client");
        }
    }
}