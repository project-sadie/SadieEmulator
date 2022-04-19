using Microsoft.Extensions.DependencyInjection;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PlayerFriendshipFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public PlayerFriendshipData CreateFriendshipData(int id, string username, string figureCode, string motto, AvatarGender gender)
    {
        return ActivatorUtilities.CreateInstance<PlayerFriendshipData>(
            _serviceProvider,
            id, 
            username,
            figureCode,
            motto,
            gender
        );
    }
    
    public PlayerFriendship CreateFriendship(
        int requestId, 
        int originId, 
        int targetId, 
        PlayerFriendshipStatus status, 
        PlayerFriendshipType type, 
        PlayerFriendshipData data)
    {
        return ActivatorUtilities.CreateInstance<PlayerFriendship>(
            _serviceProvider,
            requestId,
            originId,
            targetId,
            status, 
            type,
            data);
    }
}