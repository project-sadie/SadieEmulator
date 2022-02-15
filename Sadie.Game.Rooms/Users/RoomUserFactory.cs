using Sadie.Shared;
using Sadie.Shared.Networking;

namespace Sadie.Game.Rooms.Users;

public static class RoomUserFactory
{
    public static RoomUser Create(INetworkObject networkObject, long id, HPoint point, HDirection directionHead, HDirection direction, string username, string motto, string figureCode, string gender, long achievementScore) => 
        new RoomUser(networkObject, id, point, directionHead, direction, username, motto, figureCode, gender, achievementScore);
}