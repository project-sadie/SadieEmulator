using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;
using Sadie.Game.Players.Relationships;
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
        var amount = reader.ReadInteger();
        const int limit = 100;

        for (var i = 0; i < amount && i < limit; i++)
        {
            await ProcessAsync(client, reader);
        }
    }

    private async Task ProcessAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var playerId = player.Data.Id;
        
        var friendshipComponent = player.Data.FriendshipComponent;
        var originId = reader.ReadInteger();
            
        var request = friendshipComponent
            .Friendships
            .FirstOrDefault(x => x.OriginId == originId && x.Status == PlayerFriendshipStatus.Pending);

        if (request == null)
        {
            return;
        }
        
        if (request.TargetId != playerId)
        {
            return;
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
                const bool isOnline = true;
                var inRoom = origin.Data.CurrentRoomId != 0;

                var relationship = origin
                        .Data
                        .Relationships
                        .FirstOrDefault(x =>
                            x.TargetPlayerId == targetRequest.OriginId || x.TargetPlayerId == targetRequest.TargetId);

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
                    false,
                    relationship?.Type ?? PlayerRelationshipType.None).GetAllBytes();
                
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
                
        var relationship2 = targetOnline
            ? origin!
                .Data
                .Relationships
                .FirstOrDefault(x => x.TargetPlayerId == request.OriginId || x.TargetPlayerId == request.TargetId) : null;
        
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
                false,
                relationship2?.Type ?? PlayerRelationshipType.None).GetAllBytes();
        
        await client.WriteToStreamAsync(updateFriendWriter2);
    }
}