using Microsoft.Extensions.DependencyInjection;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipFactory(IServiceProvider serviceProvider)
{
    public PlayerFriendshipData CreateFriendshipData(int id, string username, string figureCode, string motto, AvatarGender gender)
    {
        return ActivatorUtilities.CreateInstance<PlayerFriendshipData>(
            serviceProvider,
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
        PlayerFriendshipData data)
    {
        return ActivatorUtilities.CreateInstance<PlayerFriendship>(
            serviceProvider,
            requestId,
            originId,
            targetId,
            status, 
            data);
    }
}