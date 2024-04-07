using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Players;
using Sadie.Game.Players.Balance;
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
        PlayerData data)
    {
        return ActivatorUtilities.CreateInstance<PlayerLogic>(
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
        int mode)
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

    public PlayerGameSettings CreateSettings(int systemVolume,
        int furnitureVolume,
        int traxVolume,
        bool preferOldChat,
        bool blockRoomInvites,
        bool blockCameraFollow,
        int uiFlag,
        bool showNotifications)
    {
        return ActivatorUtilities.CreateInstance<PlayerGameSettings>(
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