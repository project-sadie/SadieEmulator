using System.Collections.Concurrent;
using System.Drawing;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Shared.Extensions;

namespace Sadie.Game.Rooms.Mapping;

public class RoomTileMap : RoomTileMapHelperService, IRoomTileMap
{
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public short[,] Map { get; }
    public ConcurrentDictionary<Point, List<IRoomUnitData>> UnitMap { get; } = [];
    public short[,] ZMap { get; set; }
    public short[,] TileExistenceMap { get; set; }
    public short[,] EffectMap { get; }

    public RoomTileMap(
        string heightmap, 
        ICollection<PlayerFurnitureItemPlacementData> furnitureItems)
    {
        var heightmapLines = heightmap
            .Replace("\n", "")
            .Split("\r")
            .ToList();
        
        SizeX = heightmapLines[0].Length;
        SizeY = heightmapLines.Count;
        Size = SizeY * SizeX;
        Map = new short[SizeY, SizeX];
        ZMap = new short[SizeY, SizeX];
        TileExistenceMap = new short[SizeY, SizeX];
        EffectMap = new short[SizeY, SizeX];
        
        for (var y = 0; y < SizeY; y++)
        {
            for (var x = 0; x < SizeX; x++)
            {
                if (heightmapLines[y].Length != SizeX)
                {
                    break;
                }

                var square = heightmapLines[y][x].ToString().ToUpper();
                
                if (square == "X")
                {
                    TileExistenceMap[y, x] = 0;
                    continue;
                }

                if (!short.TryParse(square, out var height))
                {
                    height = (short) (10 + "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(square));
                }

                Map[y, x] = (short) GetTileState(x, y, furnitureItems);
                ZMap[y, x] = height;
                TileExistenceMap[y, x] = 1;

                UpdateEffectMapForTile(x, y, furnitureItems);
            }
        }
    }

    public void UpdateEffectMapForTile(int x,
        int y,
        ICollection<PlayerFurnitureItemPlacementData> furnitureItems)
    {
        var itemsOnSquare = GetItemsForPosition(x, y, furnitureItems);

        if (itemsOnSquare.Count == 0)
        {
            EffectMap[y, x] = 0;
            return;
        }
        
        var topItemOnSquare = itemsOnSquare.MaxBy(x => x.PositionZ);
        var effect = GetEffectFromInteractionType(topItemOnSquare.FurnitureItem.InteractionType);
                    
        EffectMap[y, x] = (short) effect;
    }

    public void AddUnitToMap(Point point, IRoomUnitData unit) => 
        UnitMap.GetOrInsert(point, () => []).Add(unit);

    public bool UsersAtPoint(Point point) =>
        UnitMap.ContainsKey(point) && UnitMap[point].Count > 0;

    public bool TileExists(Point point) =>
        point.X <= SizeX && 
        point.Y <= SizeY && 
        TileExistenceMap[point.Y, point.X] == 1;
}