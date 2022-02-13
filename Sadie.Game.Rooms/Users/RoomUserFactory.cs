using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public static class RoomUserFactory
{
    public static RoomUser Create(long id, HPoint point, HDirection directionHead, HDirection direction, string username, string motto, string figureCode, string gender, long achievementScore) => 
        new RoomUser(id, point, directionHead, direction, username, motto, figureCode, gender, achievementScore);
}