using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

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
        var amount = reader.ReadInt();
        const int limit = 100;

        for (var i = 0; i < amount && i < limit; i++)
        {
            var originId = reader.ReadInt();
            var request = client.Player!.FriendshipComponent.Friendships.FirstOrDefault(x => x.OriginId == originId && x.Status == PlayerFriendshipStatus.Pending);

            if (request == null)
            {
                continue;
            }
            
            if (request.TargetId != client.Player!.Id)
            {
                continue;
            }

            if (_playerRepository.TryGetPlayerById(originId, out var origin) && origin != null)
            {
                origin.FriendshipComponent.OutgoingRequestAccepted(client.Player.Id);
                
                var targetRequest = origin.
                        FriendshipComponent.
                        Friendships.
                        FirstOrDefault(x => x.OriginId == originId && x.TargetId == client.Player.Id);

                if (targetRequest != null)
                {
                    await origin.NetworkObject.WriteToStreamAsync(new PlayerUpdateFriendWriter(targetRequest, _playerRepository, _roomRepository).GetAllBytes());    
                }
            }

            await _friendshipRepository.AcceptFriendRequestAsync(originId, client.Player!.Id);
            client.Player.FriendshipComponent.AcceptIncomingRequest(originId);
            
            await client.WriteToStreamAsync(new PlayerUpdateFriendWriter(request, _playerRepository, _roomRepository).GetAllBytes());
        }
    }
}