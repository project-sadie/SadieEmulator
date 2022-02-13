using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public class RoomUser : RoomUserData
{
    public RoomUser(long id, HPoint point, HDirection directionHead, HDirection direction, string username, string motto, string figureCode, string gender, long achievementScore) : 
        base(id, point, directionHead, direction, username, motto, figureCode, gender, achievementScore)
    {
    }
}