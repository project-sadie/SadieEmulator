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
    public static List<Point> BuildPathForWalk(RoomTileMap tileMap, Point start, Point end, bool useDiagonal)
    {
        var pathfinderOptions = new PathFinderOptions
        {
            UseDiagonals = useDiagonal
        };
        
        var worldGrid = new WorldGrid(tileMap.Map);
        var pathfinder = new PathFinder(worldGrid, pathfinderOptions);

        return pathfinder
            .FindPath(start, end)
            .ToList();
    }

    public static HDirection GetDirectionForNextStep(Point current, Point next)
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

    public static List<RoomFurnitureItem> GetItemsForPosition(int x, int y, IEnumerable<RoomFurnitureItem> items)
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