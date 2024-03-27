using Microsoft.Extensions.DependencyInjection;
using Sadie.Shared.Networking;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUserFactory(IServiceProvider serviceProvider) : IRoomUserFactory
{
    public RoomUser Create(
        IRoom room,
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