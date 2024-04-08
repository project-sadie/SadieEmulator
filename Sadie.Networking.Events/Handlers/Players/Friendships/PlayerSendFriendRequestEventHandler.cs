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
        var playerData = player.Data;

        var targetUsername = eventParser.TargetUsername;
        
        if (player.Friendships.Count >= playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TooManyFriends).GetAllBytes());
            return;
        }
        
        if (targetUsername == player.Username)
        {
            return;
        }
        
        Player? targetPlayer = null;
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

        if (targetPlayer.Friendships.Count >= playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetTooManyFriends).GetAllBytes());
        }
        
        if (!targetPlayer.Data.AllowFriendRequests)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetNotAccepting).GetAllBytes());
            return;
        }

        var friendship = player
            .Friendships
            .FirstOrDefault(x => x.OriginPlayerId == targetPlayer.Id || x.TargetPlayerId == targetPlayer.Id);

        if (friendship != null)
        {
            switch (friendship.Status)
            {
                case PlayerFriendshipStatus.Pending:
                    friendship.Status = PlayerFriendshipStatus.Accepted;

                    if (targetOnline && onlineTarget != null)
                    {
                        var targetRequest = onlineTarget
                            .Friendships
                            .FirstOrDefault(x => x.OriginPlayerId == player.Id || x.TargetPlayerId == player.Id);

                        if (targetRequest != null)
                        {
                            targetRequest.Status = PlayerFriendshipStatus.Accepted;
                        }
                    }

                    await dbContext.SaveChangesAsync();
                    return;
            }
        }
        else
        {
            player.Friendships.Add(new PlayerFriendship
            {
                OriginPlayerId = player.Id,
                TargetPlayerId = targetPlayer.Id
            });
            
            if (targetOnline && onlineTarget != null)
            {
                var friendRequestWriter = new PlayerFriendRequestWriter(
                    player.Id,
                    player.Username, 
                    player.AvatarData.FigureCode).GetAllBytes();
            
                onlineTarget.Friendships.Add(new PlayerFriendship
                {
                    OriginPlayerId = player.Id,
                    TargetPlayerId = targetPlayer.Id
                });
                
                await onlineTarget.NetworkObject.WriteToStreamAsync(friendRequestWriter);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}