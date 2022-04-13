using Microsoft.Extensions.DependencyInjection;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Game.Rooms;
using Sadie.Shared.Networking;

namespace Sadie.Game.Rooms.Users;

public class RoomUserFactory : IRoomUserFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RoomUserFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

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
            _serviceProvider,
            room,
            networkObject, 
            id, 
            point, 
            directionHead, 
            direction, 
            avatarData);
    }
}