using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;
using Sadie.Game.Players.Relationships;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerAcceptFriendRequestEventHandler(
    PlayerAcceptFriendRequestEventParser eventParser,
    PlayerRepository playerRepository,
    IPlayerFriendshipRepository friendshipRepository,
    RoomRepository roomRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerAcceptFriendRequest;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        foreach (var originId in eventParser.Ids)
        {
            await AcceptAsync(client, originId);
        }
    }

    private async Task AcceptAsync(INetworkClient client, int originId)
    {
        var player = client.Player;
        var playerId = player.Data.Id;
        
        var friendshipComponent = player.FriendshipComponent;
            
        var request = friendshipComponent
            .Friendships
            .FirstOrDefault(x => x.OriginId == originId && x.Status == PlayerFriendshipStatus.Pending);

        if (request == null || request.TargetId != playerId)
        {
            return;
        }

        if (playerRepository.TryGetPlayerById(originId, out var origin) && origin != null)
        {
            var targetFriendshipComponent = origin.FriendshipComponent;
            
            targetFriendshipComponent.OutgoingRequestAccepted(playerId);
            
            var targetRequest = targetFriendshipComponent.
                Friendships.
                FirstOrDefault(x => x.OriginId == originId && x.TargetId == playerId);

            if (targetRequest != null)
            {
                const bool isOnline = true;
                var inRoom = origin.CurrentRoomId != 0;

                var relationship = origin
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
            var (roomFound, lastRoom) = roomRepository.TryGetRoomById(origin.CurrentRoomId);

            if (roomFound && lastRoom != null && lastRoom.UserRepository.TryGet(origin.Data.Id, out _))
            {
                targetInRoom = true;
            }
        }
                
        var relationship2 = targetOnline
            ? origin!
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