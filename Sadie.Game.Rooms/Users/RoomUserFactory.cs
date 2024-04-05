using Microsoft.Extensions.DependencyInjection;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Users;

public class RoomUserFactory(IServiceProvider serviceProvider) : IRoomUserFactory
{
    public RoomUser Create(
        Room room,
        INetworkObject networkObject, 
        int id, 
        HPoint point, 
        HDirection directionHead,
        HDirection direction, 
        IAvatarData avatarData,
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
            avatarData,
            controllerLevel);
    }
}