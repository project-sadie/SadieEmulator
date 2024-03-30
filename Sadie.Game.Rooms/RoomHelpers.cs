using System.Drawing;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Game.Rooms.PathFinding;
using Sadie.Game.Rooms.PathFinding.Options;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public static class RoomHelpers
{
    public static List<RoomTile> BuildTileListFromHeightMap(string heightMap, 
        IRoomFurnitureItemRepository furnitureItemRepository)
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

                var items = furnitureItemRepository
                    .Items
                    .Where(item => item.Position.X == x && item.Position.Y == y)
                    .ToList();
                
                var tile = new RoomTile(x, y, zResult ? z : 33, state, items);
                
                tiles.Add(tile);
            }
        }
        
        return tiles;
    }

    public static Queue<HPoint> BuildPathForWalk(RoomLayout layout, Point start, Point end, bool useDiagonal)
    {
        var pathfinderOptions = new PathFinderOptions
        {
            UseDiagonals = useDiagonal,
        };
        
        var worldGrid = new WorldGrid(layout.TileMap);
        var pathfinder = new PathFinder(worldGrid, pathfinderOptions);
        var route = pathfinder.FindPath(start, end).ToList();

        return new Queue<HPoint>(route.Select(x => layout.Tiles.First(y => y.Point.X == x.X && y.Point.Y == x.Y).Point)
            .Skip(1));
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

    public static void UpdateTileMapForTile(RoomTile tile, RoomLayout layout)
    {
        var topLevelItem = tile.Items.MaxBy(x => x.Position.Z);

        if (topLevelItem == null)
        {
            layout.TileMap[tile.Point.Y, tile.Point.X] = 1;
        }
        else
        {
            var furnitureItem = topLevelItem.FurnitureItem;
            var canWalkOnItem = furnitureItem.CanWalk || furnitureItem.CanSit;

            if (tile.Users.Count > 0)
            {
                canWalkOnItem = false;
            }
            
            layout.TileMap[tile.Point.Y, tile.Point.X] = (short)(canWalkOnItem ? 1 : 0);
        }
    }
}