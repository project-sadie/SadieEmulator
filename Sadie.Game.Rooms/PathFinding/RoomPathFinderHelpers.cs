using System.Drawing;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.PathFinding.ToGo;
using Sadie.Game.Rooms.PathFinding.ToGo.Options;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.PathFinding;

public class RoomPathFinderHelpers
{
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
    
    public static List<Point> BuildPathForWalk(RoomTileMap tileMap, Point start, Point end, bool useDiagonal)
    {
        var pathfinderOptions = new PathFinderOptions
        {
            UseDiagonals = useDiagonal
        };

        var worldArray = RoomTileMapHelpers.GetWorldArrayFromTileMap(tileMap, end);
        var worldGrid = new WorldGrid(worldArray);
        var pathfinder = new PathFinder(worldGrid, pathfinderOptions);

        return pathfinder
            .FindPath(start, end)
            .ToList();
    }
}