using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserLookAtEvent(IRoomRepository roomRepository) : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }

        if (roomUser!.StatusMap.ContainsKey(RoomUserStatus.Move))
        {
            return Task.CompletedTask;
        }

        var currentPoint = roomUser.Point;
        var x = reader.ReadInteger();
        var y = reader.ReadInteger();

        if (currentPoint.X == x && currentPoint.Y == y)
        {
            return Task.CompletedTask;
        }

        roomUser.LookAtPoint(new Point(x, y));
        return Task.CompletedTask;
    }
}