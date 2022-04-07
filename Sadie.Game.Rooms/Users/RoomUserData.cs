using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData
{
    public long Id { get; }
    public HPoint Point { get; protected set;  }
    public HDirection DirectionHead { get; protected set; }
    public HDirection Direction { get; protected set; }
    public AvatarData AvatarData { get; }

    public readonly Dictionary<string, string> StatusMap;
    
    protected RoomUserData(long id, HPoint point, HDirection directionHead, HDirection direction, AvatarData avatarData)
    {
        Id = id;
        Point = point;
        DirectionHead = directionHead;
        Direction = direction;
        AvatarData = avatarData;
        StatusMap = new Dictionary<string, string>();
    }
    
    protected Queue<HPoint> GoalSteps = new();
    protected HPoint? NextPoint;
    protected bool IsWalking { get; set; }
}