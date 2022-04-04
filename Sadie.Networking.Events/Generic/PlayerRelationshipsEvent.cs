using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Generic;

namespace Sadie.Networking.Events.Generic;

public class PlayerRelationshipsEvent : INetworkPacketEvent
{
    private readonly IPlayerFriendshipRepository _friendshipRepository;

    public PlayerRelationshipsEvent(IPlayerFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInt();
        var playerFriends = await _friendshipRepository.GetFriendshipRecords(playerId, PlayerFriendshipStatus.Accepted);
        
        var playerRelations = new Dictionary<int, List<PlayerFriendshipData>>
        {
            {1, playerFriends.Where(x => x.FriendshipType == PlayerFriendshipType.Lover).ToList()},
            {2, playerFriends.Where(x => x.FriendshipType == PlayerFriendshipType.Friend).ToList()},
            {3, playerFriends.Where(x => x.FriendshipType == PlayerFriendshipType.Hater).ToList()}
        };

        await client.WriteToStreamAsync(new PlayerRelationshipsWriter(playerId, playerRelations).GetAllBytes());
    }
}