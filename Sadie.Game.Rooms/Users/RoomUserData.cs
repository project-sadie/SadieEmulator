using System.Drawing;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Players;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData : RoomUnitMovementData, IRoomUserData
{
    public IPlayerLogic Player { get; }
    public DateTime LastAction { get; set; }
    public TimeSpan IdleTime { get; }
    public bool IsIdle { get; protected set; }
    public bool MoonWalking { get; set; }
    
    protected RoomUserData(IRoomLogic room, Point point, double pointZ, HDirection directionHead, HDirection direction, PlayerLogic player, TimeSpan idleTime) : base(room, point)
    {
        PointZ = pointZ;
        DirectionHead = directionHead;
        Direction = direction;
        Player = player;
        LastAction = DateTime.Now;
        IdleTime = idleTime;
    }
    
    protected Point PathGoal { get; set; }
    protected List<Point> PathPoints { get; set; } = [];
    protected bool NeedsPathCalculated { get; set; }
    protected Action? OnReachedGoal { get; set; } = null;
    protected Point? NextPoint { get; set; }
    protected int StepsWalked { get; set; }
}