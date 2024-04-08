using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Players;
using Sadie.Game.Rooms.Enums;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Users;

public class RoomUserFactory(IServiceProvider serviceProvider)
{
    public RoomUser Create(
        RoomLogic room,
        INetworkObject networkObject, 
        int id, 
        HPoint point, 
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
            directionHead, 
            direction, 
            player,
            controllerLevel);
    }
}