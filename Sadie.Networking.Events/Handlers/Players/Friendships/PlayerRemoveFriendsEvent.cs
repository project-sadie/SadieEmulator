using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerRemoveFriendsEvent(
    IPlayerRepository playerRepository,
    IPlayerFriendshipRepository friendshipRepository)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = client.Player.Data.Id;
        
        var amount = reader.ReadInteger();
        var removedIds = new List<int>();

        for (var i = 0; i < amount; i++)
        {
            var currentId = reader.ReadInteger();
            removedIds.Add(currentId);

            if (playerRepository.TryGetPlayerById(currentId, out var target) && target != null)
            {
                target.Data.FriendshipComponent.RemoveFriend(playerId);
                await target.NetworkObject.WriteToStreamAsync(new PlayerRemoveFriendsWriter([playerId]).GetAllBytes());
            }

            await friendshipRepository.DeleteFriendshipAsync(playerId, currentId);
        }

        client.Player.Data.FriendshipComponent.RemoveFriends(removedIds);
        await client.WriteToStreamAsync(new PlayerRemoveFriendsWriter(removedIds).GetAllBytes());
    }
}