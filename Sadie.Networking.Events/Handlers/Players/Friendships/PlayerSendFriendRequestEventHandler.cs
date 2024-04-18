using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerSendFriendRequestEventHandler(
    PlayerSendFriendRequestEventParser eventParser,
    PlayerRepository playerRepository,
    PlayerConstants playerConstants,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerFriendRequests;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;

        var targetUsername = eventParser.TargetUsername;
        
        if (player.GetAcceptedFriendshipCount() >= playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TooManyFriends).GetAllBytes());
            return;
        }
        
        if (targetUsername == player.Username)
        {
            return;
        }
        
        Player? targetPlayer;
        var targetOnline = false;

        if (playerRepository.TryGetPlayerByUsername(targetUsername, out var onlineTarget) && onlineTarget != null)
        {
            targetPlayer = onlineTarget;
            targetOnline = true;
        }
        else
        {
            targetPlayer = await playerRepository.TryGetPlayerByUsernameAsync(targetUsername);
        }
        
        if (targetPlayer == null)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetNotFound).GetAllBytes());
            return;
        }

        if (targetPlayer.GetAcceptedFriendshipCount() >= playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetTooManyFriends).GetAllBytes());
        }
        
        if (!targetPlayer.Data.AllowFriendRequests)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetNotAccepting).GetAllBytes());
            return;
        }

        var existingRequest = player
            .IncomingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetPlayer.Id);

        if (existingRequest is { Status: PlayerFriendshipStatus.Pending })
        {
            await AcceptPendingAsync(
                existingRequest, 
                targetOnline, 
                onlineTarget,
                player.Id);
            
            return;
        }

        var playerFriendship = new PlayerFriendship
        {
            OriginPlayerId = player.Id,
            TargetPlayerId = targetPlayer.Id
        };
            
        player.OutgoingFriendships.Add(playerFriendship);
            
        if (targetOnline && onlineTarget != null)
        {
            var friendRequestWriter = new PlayerFriendRequestWriter(
                player.Id,
                player.Username, 
                player.AvatarData.FigureCode).GetAllBytes();
            
            onlineTarget.IncomingFriendships.Add(playerFriendship);
                
            await onlineTarget.NetworkObject.WriteToStreamAsync(friendRequestWriter);
        }

        dbContext.Set<PlayerFriendship>().Add(playerFriendship);
    }

    private async Task AcceptPendingAsync(
        PlayerFriendship incomingRequest, 
        bool targetOnline, 
        PlayerLogic? onlineTarget,
        int playerId)
    {
        if (incomingRequest.Status != PlayerFriendshipStatus.Pending)
        {
            return;
        }
            
        incomingRequest.Status = PlayerFriendshipStatus.Accepted;

        if (targetOnline && onlineTarget != null)
        {
            var targetRequest = onlineTarget
                .OutgoingFriendships
                .FirstOrDefault(x => x.TargetPlayerId == playerId);

            if (targetRequest != null)
            {
                targetRequest.Status = PlayerFriendshipStatus.Accepted;
            }
        }

        await dbContext.SaveChangesAsync();
    }
}