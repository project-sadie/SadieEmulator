using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData : RoomUserAvatarData
{
    public long Id { get; }
    public HPoint Point { get; }
    public HDirection DirectionHead { get; }
    public HDirection Direction { get; }

    protected RoomUserData(long id, HPoint point, HDirection directionHead, HDirection direction, string username, string motto, string figureCode, string gender, long achievementScore) : 
        base (username, motto, figureCode, gender, achievementScore)
    {
        Id = id;
        Point = point;
        DirectionHead = directionHead;
        Direction = direction;
    }
}