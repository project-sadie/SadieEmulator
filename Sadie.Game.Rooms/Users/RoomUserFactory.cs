using Microsoft.Extensions.DependencyInjection;
using Sadie.Shared;
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
        string username, 
        string motto, 
        string figureCode, 
        string gender, 
        long achievementScore)
    {
        return ActivatorUtilities.CreateInstance<RoomUser>(
            _serviceProvider,
            room,
        networkObject, 
            id, 
            point, 
            directionHead, 
            direction, 
            username, 
            motto, 
            figureCode, 
            gender, 
            achievementScore);
    }
}