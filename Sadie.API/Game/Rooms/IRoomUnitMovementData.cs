using System.Drawing;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.API.Game.Rooms;

public interface IRoomUnitMovementData
{
    Point Point { get; set; }
    double PointZ { get; }
    HDirection DirectionHead { get; set; }
    HDirection Direction { get; set; }
    void AddStatus(string key, string value);
    void RemoveStatuses(params string[] statuses);
    List<Point> OverridePoints { get; }
}