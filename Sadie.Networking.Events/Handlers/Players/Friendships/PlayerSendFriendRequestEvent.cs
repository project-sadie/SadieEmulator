using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerSendFriendRequestEvent(
    PlayerSendFriendRequestParser parser,
    IPlayerRepository playerRepository,
    IPlayerFriendshipRepository friendshipRepository,
    PlayerConstants playerConstants)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var player = client.Player;
        var playerData = player.Data;

        var targetUsername = parser.TargetUsername;
        
        if (playerData.FriendshipComponent.Friendships.Count >= playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TooManyFriends).GetAllBytes());
            return;
        }
        
        if (targetUsername == playerData.Username)
        {
            return;
        }
        
        IPlayerData? targetData = null;
        var targetOnline = false;

        if (playerRepository.TryGetPlayerByUsername(targetUsername, out var targetPlayer) && targetPlayer != null)
        {
            targetData = targetPlayer.Data;
            targetOnline = true;
        }
        else
        {
            var (found, offlineData) = await playerRepository.TryGetPlayerDataByUsernameAsync(targetUsername);

            if (found)
            {
                targetData = offlineData!;
            }
        }
        
        if (targetData == null)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetNotFound).GetAllBytes());
            return;
        }

        var friendshipComponent = playerData.FriendshipComponent;

        if (friendshipComponent.IsFriendsWith(targetData.Id))
        {
            return;
        }
        
        if (targetData.FriendshipComponent.Friendships.Count >= playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetTooManyFriends).GetAllBytes());
        }
        
        if (!targetData.AllowFriendRequests)
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
                    await friendshipRepository.AcceptFriendRequestAsync(playerData.Id, targetData.Id);
                    friendshipComponent.AcceptIncomingRequest(targetData.Id);

                    if (targetOnline && targetPlayer != null)
                    {
                        targetPlayer.Data.FriendshipComponent.OutgoingRequestAccepted(targetData.Id);
                    }
                    return;
            }
        }
        
        await friendshipRepository.CreateFriendRequestAsync(playerData.Id, targetData.Id);
        friendshipComponent.Friendships = await friendshipRepository.GetAllRecordsForPlayerAsync(playerData.Id);

        if (targetOnline && targetPlayer != null)
        {
            var friendRequestWriter = new PlayerFriendRequestWriter(
                playerData.Id,
                playerData.Username, 
                playerData.FigureCode).GetAllBytes();
            
            targetPlayer.Data.FriendshipComponent.Friendships = await friendshipRepository.GetAllRecordsForPlayerAsync(targetData.Id);
            await targetPlayer.NetworkObject.WriteToStreamAsync(friendRequestWriter);
        }
    }
}