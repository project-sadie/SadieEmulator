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
        var playerId = reader.ReadInteger();
        var relationId = reader.ReadInteger();
        
        var friendshipComponent = client.Player.Data.FriendshipComponent;
        
        var friendship = friendshipComponent.
            Friendships.
            FirstOrDefault(x => x.OriginId == playerId || x.TargetId == playerId);

        if (friendship != null)
        {
            friendshipComponent.UpdateRelation(playerId, (PlayerFriendshipType) relationId);
            // TODO: Persist the update
            
            var isOnline = _playerRepository.TryGetPlayerById(playerId, out var onlineFriend) && onlineFriend != null;
            var inRoom = false;

            if (isOnline && onlineFriend != null)
            {
                var onlineData = onlineFriend.Data;
                var (roomFound, lastRoom) = _roomRepository.TryGetRoomById(onlineData.CurrentRoomId);

                if (roomFound && lastRoom != null && lastRoom.UserRepository.TryGet(onlineData.Id, out _))
                {
                    inRoom = true;
                }
            }
            
            await client.WriteToStreamAsync(new PlayerUpdateFriendWriter(friendship, isOnline, inRoom).GetAllBytes());
        }
    }
}