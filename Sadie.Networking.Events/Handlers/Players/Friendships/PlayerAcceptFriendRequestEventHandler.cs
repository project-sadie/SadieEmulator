using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerAcceptFriendRequestEventHandler(
    PlayerAcceptFriendRequestEventParser eventParser,
    PlayerRepository playerRepository,
    RoomRepository roomRepository,
    SadieContext dbContext)
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
        var playerId = player.Id;
        
        var request = player
            .IncomingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == originId && x.Status == PlayerFriendshipStatus.Pending);

        if (request == null || request.TargetPlayerId != playerId)
        {
            return;
        }

        request.Status = PlayerFriendshipStatus.Accepted;
        await dbContext.SaveChangesAsync();
        
        if (playerRepository.TryGetPlayerById(originId, out var origin) && origin != null)
        {
            var targetRequest = origin.
                OutgoingFriendships.
                FirstOrDefault(x => x.TargetPlayerId == playerId);

            if (targetRequest != null)
            {
                const bool isOnline = true;
                var inRoom = origin.CurrentRoomId != 0;

                var relationship = origin
                        .Relationships
                        .FirstOrDefault(x =>
                            x.TargetPlayerId == targetRequest.OriginPlayerId || x.TargetPlayerId == targetRequest.TargetPlayerId);

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
                    relationship?.TypeId ?? PlayerRelationshipType.None).GetAllBytes();
                
                await origin.NetworkObject.WriteToStreamAsync(updateFriendWriter);    
            }
        }

        var targetOnline = origin != null;
        var targetInRoom = false;

        if (targetOnline && origin != null)
        {
            var lastRoom = roomRepository.TryGetRoomById(origin.CurrentRoomId);

            if (lastRoom != null && lastRoom.UserRepository.TryGet(origin.Id, out _))
            {
                targetInRoom = true;
            }
        }
                
        var targetRelationship = targetOnline
            ? origin!
                .Relationships
                .FirstOrDefault(x => x.TargetPlayerId == request.OriginPlayerId || x.TargetPlayerId == request.TargetPlayerId) : null;
        
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
                targetRelationship?.TypeId ?? PlayerRelationshipType.None).GetAllBytes();
        
        await client.WriteToStreamAsync(updateFriendWriter2);
    }
}