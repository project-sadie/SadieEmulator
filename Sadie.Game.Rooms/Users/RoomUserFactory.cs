using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Rooms.Enums;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Users;

public class RoomUserFactory(IServiceProvider serviceProvider)
{
    public RoomUser Create(
        RoomLogic room,
        INetworkObject networkObject, 
        int id, 
        Point point, 
        double pointZ,
        HDirection directionHead,
        HDirection direction, 
        Player player,
        RoomControllerLevel controllerLevel)
    {
        return ActivatorUtilities.CreateInstance<RoomUser>(
            serviceProvider,
            room,
            networkObject, 
            id, 
            point, 
            pointZ,
            directionHead, 
            direction, 
            player,
            controllerLevel);
    }
}