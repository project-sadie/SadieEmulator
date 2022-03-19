using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserLookAtEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserLookAtEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out _, out var roomUser))
        {
            return;
        }

        if (roomUser!.StatusMap.ContainsKey(RoomUserStatus.Move))
        {
            return;
        }

        var currentPoint = roomUser.Point;
        var x = reader.ReadInt();
        var y = reader.ReadInt();

        if (currentPoint.X == x && currentPoint.Y == y)
        {
            return;
        }

        roomUser.LookAtPoint(new Point(x, y));
    }
}