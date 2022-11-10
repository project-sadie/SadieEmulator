using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Players.Friendships;

public class PlayerAcceptFriendRequestEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerFriendshipRepository _friendshipRepository;
    private readonly IRoomRepository _roomRepository;

    public PlayerAcceptFriendRequestEvent(IPlayerRepository playerRepository, IPlayerFriendshipRepository friendshipRepository, IRoomRepository roomRepository)
    {
        _playerRepository = playerRepository;
        _friendshipRepository = friendshipRepository;
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var playerId = player.Data.Id;
        
        var amount = reader.ReadInteger();
        const int limit = 100;

        var friendshipComponent = player.Data.FriendshipComponent;
        
        for (var i = 0; i < amount && i < limit; i++)
        {
            var originId = reader.ReadInteger();
            var request = friendshipComponent.Friendships.FirstOrDefault(x => x.OriginId == originId && x.Status == PlayerFriendshipStatus.Pending);

            if (request == null)
            {
                continue;
            }
            
            if (request.TargetId != playerId)
            {
                continue;
            }

            if (_playerRepository.TryGetPlayerById(originId, out var origin) && origin != null)
            {
                var targetFriendshipComponent = origin.Data.FriendshipComponent;
                
                targetFriendshipComponent.OutgoingRequestAccepted(playerId);
                
                var targetRequest = targetFriendshipComponent.
                        Friendships.
                        FirstOrDefault(x => x.OriginId == originId && x.TargetId == playerId);

                if (targetRequest != null)
                {
                    var isOnline = true;
                    var inRoom = false;

                    if (isOnline)
                    {
                        var (roomFound, lastRoom) = _roomRepository.TryGetRoomById(origin.Data.CurrentRoomId);

                        if (roomFound && lastRoom != null && lastRoom.UserRepository.TryGet(origin.Data.Id, out _))
                        {
                            inRoom = true;
                        }
                    }
                    
                    await origin.NetworkObject.WriteToStreamAsync(new PlayerUpdateFriendWriter(targetRequest, isOnline, inRoom).GetAllBytes());    
                }
            }

            await _friendshipRepository.AcceptFriendRequestAsync(originId, playerId);
            friendshipComponent.AcceptIncomingRequest(originId);

            var targetOnline = origin != null;
            var targetInRoom = false;

            if (targetOnline && origin != null)
            {
                var (roomFound, lastRoom) = _roomRepository.TryGetRoomById(origin.Data.CurrentRoomId);

                if (roomFound && lastRoom != null && lastRoom.UserRepository.TryGet(origin.Data.Id, out _))
                {
                    targetInRoom = true;
                }
            }
            
            await client.WriteToStreamAsync(new PlayerUpdateFriendWriter(request, targetOnline, targetInRoom).GetAllBytes());
        }
    }
}