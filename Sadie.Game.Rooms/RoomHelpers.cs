using System.Drawing;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.PathFinding;
using Sadie.Game.Rooms.PathFinding.Options;
using Sadie.Game.Rooms.Tiles;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public static class RoomHelpers
{
    public static List<RoomTile> BuildTileListFromHeightMap(string heightMap, 
        ICollection<RoomFurnitureItem> furnitureItems)
    {
        var heightmapLines = heightMap.Split("\n").ToList();
        var tiles = new List<RoomTile>();
        
        for (var y = 0; y < heightmapLines.Count; y++)
        {
            var currentLine = heightmapLines[y];

            for (var x = 0; x < currentLine.Length; x++)
            {
                var zResult = int.TryParse(currentLine[x].ToString(), out var z);
                
                var state = zResult ? 
                    RoomTileState.Open : 
                    RoomTileState.Closed;

                var items = GetItemsForPosition(x, y, furnitureItems);
                
                var tile = new RoomTile(x, y, zResult ? z : 0, state, items);
                
                tiles.Add(tile);
            }
        }
        
        return tiles;
    }

    public static Queue<HPoint> BuildPathForWalk(RoomTileMap tileMap, HPoint start, HPoint end, bool useDiagonal)
    {
        var pathfinderOptions = new PathFinderOptions
        {
            UseDiagonals = useDiagonal
        };
        
        var worldGrid = new WorldGrid(tileMap.Map);
        var pathfinder = new PathFinder(worldGrid, pathfinderOptions);
        var route = pathfinder.FindPath(start.ToPoint(), end.ToPoint()).ToList();
        var points = route.Select(x => tileMap.Tiles.First(y => y.Point.X == x.X && y.Point.Y == x.Y).Point);
        
        return new Queue<HPoint>(points);
    }

    public static HDirection GetDirectionForNextStep(HPoint current, HPoint next)
    {
        var rotation = HDirection.North;

        if (current.X > next.X && current.Y > next.Y)
        {
            rotation = HDirection.NorthWest;
        }
        else if (current.X < next.X && current.Y < next.Y)
        {
            rotation = HDirection.SouthEast;
        }
        else if (current.X > next.X && current.Y < next.Y)
        {
            rotation = HDirection.SouthWest;
        }
        else if (current.X < next.X && current.Y > next.Y)
        {
            rotation = HDirection.NorthEast;
        }
        else if (current.X > next.X)
        {
            rotation = HDirection.West;
        }
        else if (current.X < next.X)
        {
            rotation = HDirection.East;
        }
        else if (current.Y < next.Y)
        {
            rotation = HDirection.South;
        }
        else if (current.Y > next.Y)
        {
            rotation = HDirection.North;
        }

        return rotation;
    }

    public static void UpdateTileMapForTile(RoomTile tile, RoomTileMap tileMap)
    {
        var topLevelItem = tile.Items.MaxBy(x => x.PositionZ);

        if (topLevelItem == null)
        {
            tileMap.Map[tile.Point.Y, tile.Point.X] = 1;
        }
        else
        {
            var furnitureItem = topLevelItem.FurnitureItem;
            var canWalkOnItem = furnitureItem.CanWalk || furnitureItem.CanSit;

            if (tile.Users.Count > 0)
            {
                canWalkOnItem = false;
            }
            
            tileMap.Map[tile.Point.Y, tile.Point.X] = (short)(canWalkOnItem ? 1 : 0);
        }
    }

    public static void UpdateTileMapForTiles(List<RoomTile> tiles, RoomTileMap tileMap)
    {
        foreach (var tile in tiles)
        {
            UpdateTileMapForTile(tile, tileMap);
        }
    }

    private static List<RoomFurnitureItem> GetItemsForPosition(int x, int y, IEnumerable<RoomFurnitureItem> items)
    {
        var tileItems = new List<RoomFurnitureItem>();
        
        foreach (var item in items)
        {
            var width = 0;
            var length = 0;
            
            if (item.FurnitureItem.Type != FurnitureItemType.Floor)
            {
                continue;
            }

            switch ((int)item.Direction)
            {
                case 2 or 6:
                    width = item.FurnitureItem.TileSpanY > 0 ? item.FurnitureItem.TileSpanY : 1;
                    length = item.FurnitureItem.TileSpanX > 0 ? item.FurnitureItem.TileSpanX : 1;
                    break;
                case 0 or 4:
                    width = item.FurnitureItem.TileSpanX > 0 ? item.FurnitureItem.TileSpanX : 1;
                    length = item.FurnitureItem.TileSpanY > 0 ? item.FurnitureItem.TileSpanY : 1;
                    break;
            }
            
            if (!(x >= item.PositionX && x <= item.PositionX + width - 1 &&
                  y >= item.PositionY && y <= item.PositionY + length - 1))
            {
                continue;
            }

            tileItems.Add(item);
        }

        return tileItems;
    }
    
    public static HDirection GetOppositeDirection(int direction)
    {
        return (HDirection) direction switch
        {
            HDirection.North => HDirection.South,
            HDirection.NorthEast => HDirection.SouthWest,
            HDirection.East => HDirection.West,
            HDirection.SouthEast => HDirection.NorthWest,
            HDirection.South => HDirection.North,
            HDirection.SouthWest => HDirection.NorthEast,
            HDirection.West => HDirection.East,
            HDirection.NorthWest => HDirection.SouthEast,
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
}