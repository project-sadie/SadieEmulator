using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Players;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerFriendRequests)]
public class PlayerSendFriendRequestEventHandler(
    IPlayerRepository playerRepository,
    ServerPlayerConstants playerConstants,
    IDbContextFactory<SadieContext> dbContextFactory,
    IMapper mapper)
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
                ClientMessageId = 0,
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
            targetPlayer = mapper.Map<Player>(onlineTarget);
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
                ClientMessageId = 0,
                Error = (int) PlayerFriendshipError.TargetNotFound
            });
            return;
        }

        if (targetPlayer.GetAcceptedFriendshipCount() >= playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter
            {
                ClientMessageId = 0,
                Error = (int) PlayerFriendshipError.TargetTooManyFriends
            });
            return;
        }
        
        if (!targetPlayer.Data.AllowFriendRequests)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter
            {
                ClientMessageId = 0,
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

        await SendRequestAsync(
            mapper.Map<Player>(player),
            targetPlayer,
            targetOnline,
            onlineTarget);
    }

    private async Task AcceptPendingAsync(
        PlayerFriendship incomingRequest, 
        bool targetOnline, 
        IPlayerLogic? onlineTarget,
        long playerId)
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

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.SaveChangesAsync();
    }

    private async Task SendRequestAsync(
        Player player,
        Player targetPlayer,
        bool targetOnline,
        IPlayerLogic? onlineTarget)
    {
        var playerFriendship = new PlayerFriendship
        {
            OriginPlayerId = player.Id,
            TargetPlayerId = targetPlayer.Id,
            Status = PlayerFriendshipStatus.Pending,
            CreatedAt = DateTime.Now
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

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Set<PlayerFriendship>().Add(playerFriendship);
        await dbContext.SaveChangesAsync();
    }
}