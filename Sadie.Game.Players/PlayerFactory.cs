using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Players;

namespace Sadie.Game.Players;

public class PlayerFactory(IServiceProvider serviceProvider)
{
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