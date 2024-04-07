using Sadie.Game.Rooms.Enums;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUserFactory
{
    RoomUser Create(
        RoomLogic room,
        INetworkObject networkObject, 
        int id, HPoint point, 
        HDirection directionHead,
        HDirection direction, 
        IAvatarData avatarData,
        RoomControllerLevel controllerLevel);
}