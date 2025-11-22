using System.Drawing;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms.Users;
using Sadie.Shared.Attributes;

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