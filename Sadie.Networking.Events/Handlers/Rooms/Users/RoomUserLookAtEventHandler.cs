using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.Enums.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserLookAt)]
public class RoomUserLookAtEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int X { get; init; }
    public int Y { get; init; }
    
    public Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }
        
        var currentPoint = roomUser.Point;
        
        if (roomUser.StatusMap.ContainsKey(RoomUserStatus.Lay) || 
            roomUser.IsWalking ||
            currentPoint.X == X && currentPoint.Y == Y)
        {
            return Task.CompletedTask;
        }

        roomUser.LookAtPoint(new Point(X, Y));
        return Task.CompletedTask;
    }
}