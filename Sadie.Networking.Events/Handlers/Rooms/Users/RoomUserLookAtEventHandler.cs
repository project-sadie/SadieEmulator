using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerIds.RoomUserLookAt)]
public class RoomUserLookAtEventHandler(RoomUserLookAtEventParser eventParser, RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }

        if (roomUser.IsWalking)
        {
            return Task.CompletedTask;
        }

        var currentPoint = roomUser.Point;
        var x = eventParser.X;
        var y = eventParser.Y;

        if (currentPoint.X == x && currentPoint.Y == y)
        {
            return Task.CompletedTask;
        }

        roomUser.LookAtPoint(new Point(x, y));
        return Task.CompletedTask;
    }
}