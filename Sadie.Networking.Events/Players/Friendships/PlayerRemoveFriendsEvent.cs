using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Players.Friendships;

public class PlayerRemoveFriendsEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerFriendshipRepository _friendshipRepository;

    public PlayerRemoveFriendsEvent(IPlayerRepository playerRepository, IPlayerFriendshipRepository friendshipRepository)
    {
        _playerRepository = playerRepository;
        _friendshipRepository = friendshipRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = client.Player.Data.Id;
        
        var amount = reader.ReadInteger();
        var removedIds = new List<int>();

        for (var i = 0; i < amount; i++)
        {
            var currentId = reader.ReadInteger();
            removedIds.Add(currentId);

            if (_playerRepository.TryGetPlayerById(currentId, out var target) && target != null)
            {
                target.Data.FriendshipComponent.RemoveFriend(playerId);
                await target.NetworkObject.WriteToStreamAsync(new PlayerRemoveFriendsWriter(new List<int> { playerId }).GetAllBytes());
            }

            await _friendshipRepository.DeleteFriendshipAsync(playerId, currentId);
        }

        client.Player.Data.FriendshipComponent.RemoveFriends(removedIds);
        await client.WriteToStreamAsync(new PlayerRemoveFriendsWriter(removedIds).GetAllBytes());
    }
}