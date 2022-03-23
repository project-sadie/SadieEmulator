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
        long id, HPoint point, 
        HDirection directionHead,
        HDirection direction, 
        IAvatarData avatarData)
    {
        return ActivatorUtilities.CreateInstance<RoomUser>(
            _serviceProvider,
            room,
            room.UserRepository,
        networkObject, 
            id, 
            point, 
            directionHead, 
            direction, 
            avatarData);
    }
}