using System.Drawing;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.API.Game.Rooms.Unit;

public interface IRoomUnitMovementData
{
    Point Point { get; set; }
    double PointZ { get; }
    HDirection DirectionHead { get; set; }
    HDirection Direction { get; set; }
    bool CanWalk { get; set; }
    void AddStatus(string key, string value);
    void RemoveStatuses(params string[] statuses);
    List<Point> OverridePoints { get; }
    void ClearWalking(bool reachedGoal = true);
    void WalkToPoint(Point point, Action? onReachedGoal = null);
}