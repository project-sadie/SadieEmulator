using Sadie.Shared;
using Sadie.Shared.Networking;

namespace Sadie.Game.Rooms.Users;

public class RoomUser : RoomUserData
{
    public INetworkObject NetworkObject { get; }

    public RoomUser(INetworkObject networkObject, long id, HPoint point, HDirection directionHead, HDirection direction, string username, string motto, string figureCode, string gender, long achievementScore) : 
        base(id, point, directionHead, direction, username, motto, figureCode, gender, achievementScore)
    {
        NetworkObject = networkObject;
    }
}