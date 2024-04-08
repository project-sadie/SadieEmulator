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
    PlayerConstants playerConstants)
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

        var friendshipComponent = player.FriendshipComponent;

        if (friendshipComponent.IsFriendsWith(targetPlayer.Id))
        {
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

        var playerFriends = friendshipComponent.Friendships;
        var friendship = playerFriends.FirstOrDefault(x => x.TargetData.Username == targetUsername);

        if (friendship != null)
        {
            switch (friendship.Status)
            {
                case PlayerFriendshipStatus.Accepted:
                    return;
                case PlayerFriendshipStatus.Pending:
                    await friendshipRepository.AcceptFriendRequestAsync(playerData.Id, targetPlayer.Id);
                    friendshipComponent.AcceptIncomingRequest(targetPlayer.Id);

                    if (targetOnline && onlineTarget != null)
                    {
                        onlineTarget.FriendshipComponent.OutgoingRequestAccepted(targetPlayer.Id);
                    }
                    return;
            }
        }
        
        await friendshipRepository.CreateFriendRequestAsync(playerData.Id, targetPlayer.Id);
        friendshipComponent.Friendships = await friendshipRepository.GetAllRecordsForPlayerAsync(playerData.Id);

        if (targetOnline && onlineTarget != null)
        {
            var friendRequestWriter = new PlayerFriendRequestWriter(
                playerData.Id,
                player.Username, 
                player.AvatarData.FigureCode).GetAllBytes();
            
            onlineTarget.Friendships = await friendshipRepository.GetAllRecordsForPlayerAsync(targetPlayer.Id);
            await onlineTarget.NetworkObject.WriteToStreamAsync(friendRequestWriter);
        }
    }
}