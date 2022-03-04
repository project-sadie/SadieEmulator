using System.Drawing;
using AStar;
using AStar.Options;
using Sadie.Shared;

namespace Sadie.Game.Rooms;

public static class RoomHelpers
{
    public static List<RoomTile> BuildTileListFromHeightMap(List<string> heightmapLines)
    {
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
                
                var tile = new RoomTile(x, y, zResult ? z : 33, state);
                
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
}