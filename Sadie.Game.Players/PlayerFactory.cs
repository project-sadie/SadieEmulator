using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public class PlayerFactory(IServiceProvider serviceProvider) : IPlayerFactory
{
    public IPlayerBalance CreateBalance(long credits, long pixels, long seasonal, long gotwPoints)
    {
        return ActivatorUtilities.CreateInstance<PlayerBalance>(
            serviceProvider, 
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
            serviceProvider,
            networkObject,
            data);
    }

    public PlayerNavigatorSettings CreateNavigatorSettings(
        int windowX, 
        int windowY, 
        int windowWidth, 
        int windowHeight, 
        bool openSearches, 
        int unknown1)
    {
        return ActivatorUtilities.CreateInstance<PlayerNavigatorSettings>(
            serviceProvider,
            windowX,
            windowY,
            windowWidth,
            windowHeight,
            openSearches,
            0);
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
            serviceProvider,
            systemVolume,
            furnitureVolume,
            traxVolume,
            preferOldChat,
            blockRoomInvites,
            blockCameraFollow,
            uiFlag,
            showNotifications);
    }
}