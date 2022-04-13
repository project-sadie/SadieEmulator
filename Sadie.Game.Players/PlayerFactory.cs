using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public class PlayerFactory : IPlayerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PlayerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPlayerBalance CreateBalance(long credits, long pixels, long seasonal, long gotwPoints)
    {
        return ActivatorUtilities.CreateInstance<PlayerBalance>(
            _serviceProvider, 
            credits,
            pixels,
            seasonal,
            gotwPoints);
    }
    
    public IPlayer Create(
        INetworkObject networkObject, 
        IPlayerData data)
    {
        return ActivatorUtilities.CreateInstance<Player>(
            _serviceProvider,
            networkObject,
            data);
    }

    public PlayerNavigatorSettings CreateNavigatorSettings(int windowX, int windowY, int windowWidth, int windowHeight, bool openSearches, int unknown1)
    {
        return ActivatorUtilities.CreateInstance<PlayerNavigatorSettings>(
            _serviceProvider,
            windowX,
            windowY,
            windowWidth,
            windowHeight,
            openSearches);
    }

    public PlayerSettings CreateSettings(int systemVolume,
        int furnitureVolume,
        int traxVolume,
        bool preferOldChat,
        bool blockRoomInvites,
        bool blockCameraFollow,
        int uiFlag,
        bool showNotifications)
    {
        return ActivatorUtilities.CreateInstance<PlayerSettings>(
            _serviceProvider,
            systemVolume,
            furnitureVolume,
            traxVolume,
            preferOldChat,
            blockRoomInvites,
            blockCameraFollow,
            uiFlag,
            showNotifications);
    }

    public PlayerFriendshipComponent CreatePlayerFriendshipComponent(int playerId, List<PlayerFriendship> friendships)
    {
        return ActivatorUtilities.CreateInstance<PlayerFriendshipComponent>(
            _serviceProvider,
            playerId,
            friendships);
    }
}