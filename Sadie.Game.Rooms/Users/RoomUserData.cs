using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData : IRoomUserData
{
    public HPoint Point { get; protected set;  }
    public HDirection DirectionHead { get; protected set; }
    public HDirection Direction { get; protected set; }
    public AvatarData AvatarData { get; }
    public Dictionary<string, string> StatusMap { get; }
    public DateTime LastAction { get; set; }
    public TimeSpan IdleTime { get; }
    public bool IsIdle { get; set; }
    
    protected RoomUserData(HPoint point, HDirection directionHead, HDirection direction, AvatarData avatarData, TimeSpan idleTime)
    {
        Point = point;
        DirectionHead = directionHead;
        Direction = direction;
        AvatarData = avatarData;
        StatusMap = new Dictionary<string, string>();
        LastAction = DateTime.Now;
        IdleTime = idleTime;
    }
    
    protected Queue<HPoint> GoalSteps = new();
    protected HPoint? NextPoint;
    protected bool IsWalking { get; set; }
}