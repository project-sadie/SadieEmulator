using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData : RoomUserAvatarData
{
    public long Id { get; }
    public HPoint Point { get; }
    public HDirection Direction { get; }

    protected RoomUserData(long id, HPoint point, HDirection direction, string username, string motto, string figureCode, string gender, long achievementScore) : 
        base (username, motto, figureCode, gender, achievementScore)
    {
        Id = id;
        Point = point;
        Direction = direction;
    }
}