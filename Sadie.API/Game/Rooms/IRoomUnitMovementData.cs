using System.Drawing;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.API.Game.Rooms;

public interface IRoomUnitMovementData
{
    Point Point { get; }
    double PointZ { get; }
    HDirection DirectionHead { get; set; }
    HDirection Direction { get; set; }
}