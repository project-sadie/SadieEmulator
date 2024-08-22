using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserWalk)]
public class RoomUserWalkEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }

        if (!roomUser.CanWalk)
        {
            return Task.CompletedTask;
        }
        
        roomUser.WalkToPoint(new Point(X, Y));
        
        return Task.CompletedTask;
    }
}