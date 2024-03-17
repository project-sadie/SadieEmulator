using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Players.Friendships;

public class PlayerAcceptFriendRequestEvent(
    IPlayerRepository playerRepository,
    IPlayerFriendshipRepository friendshipRepository,
    IRoomRepository roomRepository)
    : INetworkPacketEvent
{
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
            
            var request = friendshipComponent
                .Friendships
                .FirstOrDefault(x => x.OriginId == originId && x.Status == PlayerFriendshipStatus.Pending);

            if (request == null)
            {
                continue;
            }
            
            if (request.TargetId != playerId)
            {
                continue;
            }

            if (playerRepository.TryGetPlayerById(originId, out var origin) && origin != null)
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
                        var (roomFound, lastRoom) = roomRepository.TryGetRoomById(origin.Data.CurrentRoomId);

                        if (roomFound && lastRoom != null && lastRoom.UserRepository.TryGet(origin.Data.Id, out _))
                        {
                            inRoom = true;
                        }
                    }

                    var updateFriendWriter = new PlayerUpdateFriendWriter(
                        0, 
                        1, 
                        0, 
                        targetRequest, 
                        isOnline, 
                        inRoom, 
                        0,
                        "", 
                        "", 
                        false, 
                        false, 
                        false).GetAllBytes();
                    
                    await origin.NetworkObject.WriteToStreamAsync(updateFriendWriter);    
                }
            }

            await friendshipRepository.AcceptFriendRequestAsync(originId, playerId);
            friendshipComponent.AcceptIncomingRequest(originId);

            var targetOnline = origin != null;
            var targetInRoom = false;

            if (targetOnline && origin != null)
            {
                var (roomFound, lastRoom) = roomRepository.TryGetRoomById(origin.Data.CurrentRoomId);

                if (roomFound && lastRoom != null && lastRoom.UserRepository.TryGet(origin.Data.Id, out _))
                {
                    targetInRoom = true;
                }
            }

            var updateFriendWriter2 = new PlayerUpdateFriendWriter(
                    0, 
                    1, 
                    0, 
                    request, 
                    targetOnline,
                    targetInRoom, 
                    0, 
                    "", 
                    "", 
                    false, 
                    false,
                    false).GetAllBytes();
            
            await client.WriteToStreamAsync(updateFriendWriter2);
        }
    }
}