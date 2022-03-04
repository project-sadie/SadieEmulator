using Sadie.Shared;
using Sadie.Shared.Networking;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUserFactory
{
    RoomUser Create(
        IRoom room,
        INetworkObject networkObject, 
        long id, HPoint point, 
        HDirection directionHead,
        HDirection direction, 
        string username, 
        string motto, 
        string figureCode, 
        string gender, 
        long achievementScore);
}