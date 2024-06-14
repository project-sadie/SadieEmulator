using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerIds.RoomUserWalk)]
public class RoomUserWalkEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
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
        
        roomUser.LastAction = DateTime.Now;
        roomUser.WalkToPoint(new Point(X, Y));
        
        return Task.CompletedTask;
    }
}