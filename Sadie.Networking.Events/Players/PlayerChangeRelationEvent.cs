using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Players;

public class PlayerChangeRelationEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRoomRepository _roomRepository;

    public PlayerChangeRelationEvent(IPlayerRepository playerRepository, IRoomRepository roomRepository)
    {
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
    }
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInt();
        var relationId = reader.ReadInt();
        
        var friendshipComponent = client.Player.Data.FriendshipComponent;
        
        var friendship = friendshipComponent.
            Friendships.
            FirstOrDefault(x => x.OriginId == playerId || x.TargetId == playerId);

        if (friendship != null)
        {
            friendshipComponent.UpdateRelation(playerId, (PlayerFriendshipType) relationId);
            await client.WriteToStreamAsync(new PlayerUpdateFriendWriter(friendship, _playerRepository, _roomRepository).GetAllBytes());
        }
    }
}