using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Players;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerFriendRequests)]
public class PlayerSendFriendRequestEventHandler(
    PlayerRepository playerRepository,
    ServerPlayerConstants playerConstants,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public string? TargetUsername { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;
        
        if (player.GetAcceptedFriendshipCount() >= playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter
            {
                Unknown1 = 0,
                Error = (int) PlayerFriendshipError.TooManyFriends
            });
            return;
        }
        
        if (TargetUsername == player.Username)
        {
            return;
        }
        
        Player? targetPlayer;
        var targetOnline = false;
        var onlineTarget = playerRepository.GetPlayerLogicByUsername(TargetUsername);
        
        if (onlineTarget != null)
        {
            targetPlayer = onlineTarget;
            targetOnline = true;
        }
        else
        {
            targetPlayer = await playerRepository.GetPlayerByUsernameAsync(TargetUsername);
        }
        
        if (targetPlayer == null)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter
            {
                Unknown1 = 0,
                Error = (int) PlayerFriendshipError.TargetNotFound
            });
            return;
        }

        if (targetPlayer.GetAcceptedFriendshipCount() >= playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter
            {
                Unknown1 = 0,
                Error = (int) PlayerFriendshipError.TargetTooManyFriends
            });
            return;
        }
        
        if (!targetPlayer.Data.AllowFriendRequests)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter
            {
                Unknown1 = 0,
                Error = (int) PlayerFriendshipError.TargetNotAccepting
            });
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
            TargetPlayerId = targetPlayer.Id,
            Status = PlayerFriendshipStatus.Pending
        };
            
        player.OutgoingFriendships.Add(playerFriendship);
            
        if (targetOnline && onlineTarget != null)
        {
            var friendRequestWriter = new PlayerFriendRequestWriter
            {
                Id = player.Id,
                Username = player.Username,
                FigureCode = player.AvatarData.FigureCode
            };
            
            onlineTarget.IncomingFriendships.Add(playerFriendship);
                
            await onlineTarget.NetworkObject.WriteToStreamAsync(friendRequestWriter);
        }

        dbContext.Set<PlayerFriendship>().Add(playerFriendship);
        await dbContext.SaveChangesAsync();
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