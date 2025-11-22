using System.Drawing;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserWalk)]
public class RoomUserWalkEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int X { get; init; }
    public int Y { get; init; }
    
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