using System.Drawing;

namespace Sadie.Game.Rooms.PathFinding
{
    public static class PositionExtensions
    {
        public static Position ToPosition(this Point point)
        {
            return new Position(point.Y, point.X);
        }
    }
}