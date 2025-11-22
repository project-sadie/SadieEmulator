using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Enums.Miscellaneous;

namespace Sadie.Game.Rooms.Users;

public class RoomUserFactory(IServiceProvider serviceProvider) : IRoomUserFactory
{
    public IRoomUser Create(
        IRoomLogic room,
        INetworkObject networkObject, 
        Point point, 
        double pointZ,
        HDirection directionHead,
        HDirection direction, 
        IPlayerLogic player,
        RoomControllerLevel controllerLevel)
    {
        return ActivatorUtilities.CreateInstance<RoomUser>(
            serviceProvider,
            room,
            networkObject, 
            point, 
            pointZ,
            directionHead, 
            direction, 
            player,
            controllerLevel);
    }
}