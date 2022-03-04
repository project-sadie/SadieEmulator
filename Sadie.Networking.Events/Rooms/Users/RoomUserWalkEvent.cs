using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserWalkEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserWalkEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var x = reader.ReadInt();
        var y = reader.ReadInt();
        
        var tile = room.Layout.Tiles.FirstOrDefault(z => z.Point.X == x && z.Point.Y == y);
        
        if (tile == null)
        {
            return;
        }

        var point = tile.Point;
        var useDiagonal = true;

        roomUser!.WalkToPoint(new Point(point.X, point.Y), useDiagonal);
    }
}