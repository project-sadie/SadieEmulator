using Microsoft.Extensions.DependencyInjection;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Game.Rooms;
using Sadie.Shared.Networking;

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
        IAvatarData avatarData)
    {
        return ActivatorUtilities.CreateInstance<RoomUser>(
            serviceProvider,
            room,
            networkObject, 
            id, 
            point, 
            directionHead, 
            direction, 
            avatarData);
    }
}