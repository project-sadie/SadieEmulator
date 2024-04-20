using System.Drawing;
using Sadie.Game.Players;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData : IRoomUserData
{
    public Point Point { get; protected set;  }
    public double PointZ { get; protected set;  }
    public HDirection DirectionHead { get; set; }
    public HDirection Direction { get; set; }
    public PlayerLogic Player { get; }
    public Dictionary<string, string> StatusMap { get; }
    public DateTime LastAction { get; set; }
    public TimeSpan IdleTime { get; }
    public bool IsIdle { get; protected set; }
    public bool NeedsStatusUpdate { get; set; }
    public bool IsWalking { get; set; }
    
    protected RoomUserData(Point point, double pointZ, HDirection directionHead, HDirection direction, PlayerLogic player, TimeSpan idleTime)
    {
        Point = point;
        PointZ = pointZ;
        DirectionHead = directionHead;
        Direction = direction;
        Player = player;
        StatusMap = new Dictionary<string, string>();
        LastAction = DateTime.Now;
        IdleTime = idleTime;
    }
    
    protected Point PathGoal { get; set; }
    protected List<Point> PathPoints { get; set; } = [];
    protected bool NeedsPathCalculated { get; set; }
    protected Point? NextPoint { get; set; }
    protected int StepsWalked { get; set; }
}