using Sadie.Shared.Networking;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUserFactory
{
    RoomUser Create(
        IRoom room,
        INetworkObject networkObject, 
        int id, HPoint point, 
        HDirection directionHead,
        HDirection direction, 
        IAvatarData avatarData,
        RoomControllerLevel controllerLevel);
}