using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Game.Rooms;
using Sadie.Shared.Networking;

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