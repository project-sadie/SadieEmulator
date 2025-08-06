using System.Drawing;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Pathfinding;
using Sadie.Enums.Miscellaneous;
using Sadie.Game.Rooms.PathFinding.ToGo;
using Sadie.Game.Rooms.PathFinding.ToGo.Options;

namespace Sadie.Game.Rooms.PathFinding;

public class RoomPathFinderHelperService : IRoomPathFinderHelperService
{
    public HDirection GetDirectionForNextStep(Point current, Point next)
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
    
    public List<Point> BuildPathForWalk(IRoomTileMap tileMap,
        Point start,
        Point end,
        bool useDiagonal,
        List<Point> overridePoints)
    {
        var pathfinderOptions = new PathFinderOptions
        {
            UseDiagonals = useDiagonal
        };

        var worldArray = tileMap.GetWorldArrayFromTileMap(tileMap, end, overridePoints);
        var worldGrid = new WorldGrid(worldArray);
        var pathfinder = new PathFinder(worldGrid, pathfinderOptions);

        return pathfinder
            .FindPath(start, end)
            .ToList();
    }
}