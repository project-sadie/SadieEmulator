using System.Drawing;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.Enums.Unsorted;

namespace Sadie.API.Game.Rooms.Pathfinding;

public interface IRoomPathFinderHelperService
{
    HDirection GetDirectionForNextStep(Point current, Point next);

    List<Point> BuildPathForWalk(IRoomTileMap tileMap,
        Point start,
        Point end,
        bool useDiagonal,
        List<Point> overridePoints);
}