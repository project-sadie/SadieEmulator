using System.Drawing;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Shared.Extensions;

public static class PointExtensions
{
    public static Point ToPoint(this HPoint point)
    {
        return new Point(point.X, point.Y);
    }
}