using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserLookAtEventHandler(RoomUserLookAtEventParser eventParser, IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserLookAt;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }

        if (roomUser!.StatusMap.ContainsKey(RoomUserStatus.Move))
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