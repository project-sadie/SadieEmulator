using System.Collections.Concurrent;
using System.Drawing;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API.Game.Rooms.Mapping;

public interface IRoomTileMap
{
    int SizeX { get; }
    int SizeY { get; }
    int Size { get; }
    short[,] Map { get; }
    ConcurrentDictionary<Point, List<IRoomUser>> UserMap { get; }
    ConcurrentDictionary<Point, List<IRoomBot>> BotMap { get; }
    short[,] EffectMap { get; }
    short[,] ZMap { get; set; }
    short[,] TileExistenceMap { get; set; }
    void UpdateEffectMapForTile(int x, int y, ICollection<PlayerFurnitureItemPlacementData> furnitureItems);
    void AddUserToMap(Point point, IRoomUser user);
    void AddBotToMap(Point point, IRoomBot bot);
    bool UsersAtPoint(Point point);
    bool TileExists(Point point);
}