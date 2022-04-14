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
        var playerId = reader.ReadInteger();
        var playerFriends = await _friendshipRepository.GetFriendsForPlayerAsync(playerId);
        
        var playerRelations = new Dictionary<int, List<PlayerFriendshipData>>
        {
            {1, playerFriends.Where(x => x.Type == PlayerFriendshipType.Lover).Select(x => x.TargetData).ToList()},
            {2, playerFriends.Where(x => x.Type == PlayerFriendshipType.Friend).Select(x => x.TargetData).ToList()},
            {3, playerFriends.Where(x => x.Type == PlayerFriendshipType.Hater).Select(x => x.TargetData).ToList()}
        };

        await client.WriteToStreamAsync(new PlayerRelationshipsWriter(playerId, playerRelations).GetAllBytes());
    }
}