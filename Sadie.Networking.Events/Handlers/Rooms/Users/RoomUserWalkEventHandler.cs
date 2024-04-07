using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserWalkEventHandler(RoomUserWalkEventParser eventParser, RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserWalk;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return Task.CompletedTask;
        }
        
        roomUser.LastAction = DateTime.Now;
        
        var tile = room!.TileMap.Tiles.FirstOrDefault(z => z.Point.X == eventParser.X && z.Point.Y == eventParser.Y);
        
        if (tile == null)
        {
            return Task.CompletedTask;
        }

        var point = tile.Point;

        roomUser!.WalkToPoint(new Point(point.X, point.Y), room.Settings.WalkDiagonal);
        return Task.CompletedTask;
    }
}