using System.Collections.Concurrent;
using System.Drawing;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API.Game.Rooms.Mapping;

public interface IRoomTileMap : IRoomTileMapHelperService
{
    int SizeX { get; }
    int SizeY { get; }
    int Size { get; }
    short[,] Map { get; }
    ConcurrentDictionary<Point, List<IRoomUnitData>> UnitMap { get; }
    short[,] EffectMap { get; }
    short[,] ZMap { get; set; }
    short[,] TileExistenceMap { get; set; }
    void UpdateEffectMapForTile(int x, int y, ICollection<PlayerFurnitureItemPlacementData> furnitureItems);
    void AddUnitToMap(Point point, IRoomUnitData unit);
    bool UsersAtPoint(Point point);
    bool TileExists(Point point);
}