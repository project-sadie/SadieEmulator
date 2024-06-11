using System.Collections.Concurrent;
using System.Drawing;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Users;

namespace Sadie.API.Game.Rooms.Mapping;

public interface IRoomTileMap
{
    int SizeX { get; }
    int SizeY { get; }
    int Size { get; }
    short[,] SquareStateMap { get; }
    short[,] Map { get; }
    ConcurrentDictionary<Point, List<IRoomUser>> UserMap { get; }
    ConcurrentDictionary<Point, List<IRoomBot>> BotMap { get; }
    void AddUserToMap(Point point, IRoomUser user);
    void AddBotToMap(Point point, IRoomBot bot);
}