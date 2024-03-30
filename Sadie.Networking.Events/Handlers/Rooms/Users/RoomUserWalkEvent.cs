using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserWalkEvent(RoomUserWalkParser parser, IRoomRepository roomRepository) : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return Task.CompletedTask;
        }
        
        roomUser.LastAction = DateTime.Now;
        
        var tile = room!.Layout.Tiles.FirstOrDefault(z => z.Point.X == parser.X && z.Point.Y == parser.Y);
        
        if (tile == null)
        {
            return Task.CompletedTask;
        }

        var point = tile.Point;

        roomUser!.WalkToPoint(new Point(point.X, point.Y), room.Settings.CanWalkDiagonal);
        return Task.CompletedTask;
    }
}