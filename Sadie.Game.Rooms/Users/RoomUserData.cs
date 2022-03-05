using System.Drawing;
using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData : RoomUserAvatarData
{
    public long Id { get; }
    public HPoint Point { get; protected set;  }
    public HDirection DirectionHead { get; protected set; }
    public HDirection Direction { get; protected set; }

    public readonly Dictionary<string, string> StatusMap;

    protected RoomUserData(long id, HPoint point, HDirection directionHead, HDirection direction, string username, string motto, string figureCode, string gender, long achievementScore) : 
        base (username, motto, figureCode, gender, achievementScore)
    {
        Id = id;
        Point = point;
        DirectionHead = directionHead;
        Direction = direction;
        StatusMap = new Dictionary<string, string>();
    }
    
    protected Queue<HPoint> GoalSteps = new();
    protected HPoint? NextPoint;
    protected bool IsWalking;
}