using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Players.Effects;
using Sadie.Game.Players.Packets;
using Sadie.Game.Players.Relationships;
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
        var playerId = playerData.Id;
            
        client.Player = player;
        
        if (!playerRepository.TryAddPlayer(player))
        {
            logger.LogError($"Player {playerId} could not be registered");
            await DisconnectAsync(client.Guid);
            return;
        }
            
        logger.LogInformation($"Player '{playerData.Username}' has logged in");
        await playerRepository.UpdateOnlineStatusAsync(playerId, true);

        player.Data.LastOnline = DateTime.Now;
        player.Authenticated = true;

        await SendExtraPacketsAsync(client, player);
    }

    private async Task SendExtraPacketsAsync(INetworkObject networkObject, IPlayer player)
    {
        var playerData = player.Data;
        var subscriptions = playerData.Subscriptions;
        
        await networkObject.WriteToStreamAsync(new SecureLoginWriter().GetAllBytes());
        await networkObject.WriteToStreamAsync(new NoobnessLevelWriter(1).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerHomeRoomWriter(playerData.HomeRoom, playerData.HomeRoom).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerEffectListWriter(new List<PlayerEffect>()).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerClothingListWriter().GetAllBytes());
        
        await networkObject.WriteToStreamAsync(new PlayerPermissionsWriter(
            subscriptions.Exists(x => x.Name == "HABBO_CLUB") ? 1 : 0,
            2,
            true).GetAllBytes());
        
        await networkObject.WriteToStreamAsync(new PlayerStatusWriter(true, false, true).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerNavigatorSettingsWriter(playerData.NavigatorSettings).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerNotificationSettingsWriter(playerData.Settings.ShowNotifications).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerAchievementScoreWriter(playerData.AchievementScore).GetAllBytes());

        foreach (var subscription in subscriptions)
        {
            var tillExpire = subscription.Expires - subscription.Started;
            var daysLeft = (int) tillExpire.TotalDays;
            var minutesLeft = (int) tillExpire.TotalMinutes;
            var minutesSinceMod = (int)(DateTime.Now - player.State.LastSubscriptionModification).TotalMinutes;
            
            await networkObject.WriteToStreamAsync(new PlayerSubscriptionWriter(
                subscription.Name,
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

        if (playerData.Permissions.Contains("moderation_tools"))
        {
            await networkObject.WriteToStreamAsync(new ModerationToolsWriter().GetAllBytes());
        }
        
        await playerRepository.UpdateMessengerStatusForFriends(playerData.Id,
            playerData.FriendshipComponent.Friendships, true, playerData.CurrentRoomId != 0);

        foreach (var friend in playerData.FriendshipComponent.Friendships)
        {
            var isOnline = playerRepository.TryGetPlayerById(friend.TargetData.Id, out var friendPlayer) && friendPlayer != null;
            var isInRoom = isOnline && friendPlayer!.Data.CurrentRoomId != 0;
                    
            var relationship = isOnline
                ? friendPlayer!
                    .Data
                    .Relationships
                    .FirstOrDefault(x => x.TargetPlayerId == friend.OriginId || x.TargetPlayerId == friend.TargetId) : null;

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
                relationship?.Type ?? PlayerRelationshipType.None).GetAllBytes());   
        }
    }

    private bool ValidateSso(string sso) => 
        !string.IsNullOrEmpty(sso) && sso.Length >= constants.MinSsoLength;

    public async Task DisconnectAsync(Guid guid)
    {
        if (!await networkClientRepository.TryRemoveAsync(guid))
        {
            logger.LogError("Failed to remove network client");
        }
    }
}