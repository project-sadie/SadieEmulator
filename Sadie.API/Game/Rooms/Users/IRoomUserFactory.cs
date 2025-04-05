using System.Drawing;
using Sadie.API.Game.Players;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Unsorted;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUserFactory
{
    IRoomUser Create(
        IRoomLogic room,
        INetworkObject networkObject, 
        Point point, 
        double pointZ,
        HDirection directionHead,
        HDirection direction, 
        IPlayerLogic player,
        RoomControllerLevel controllerLevel);
}