using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserWalkEvent(IRoomRepository roomRepository) : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return Task.CompletedTask;
        }
        
        roomUser.LastAction = DateTime.Now;
        
        var x = reader.ReadInteger();
        var y = reader.ReadInteger();
        
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