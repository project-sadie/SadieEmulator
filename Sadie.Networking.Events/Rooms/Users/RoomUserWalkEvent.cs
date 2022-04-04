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

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return Task.CompletedTask;
        }
        
        var x = reader.ReadInt();
        var y = reader.ReadInt();
        
        var tile = room!.Layout.Tiles.FirstOrDefault(z => z.Point.X == x && z.Point.Y == y);
        
        if (tile == null)
        {
            return Task.CompletedTask;
        }

        var point = tile.Point;

        roomUser!.WalkToPoint(new Point(point.X, point.Y), room.Settings.CanWalkDiagonal);
        return Task.CompletedTask;
    }
}