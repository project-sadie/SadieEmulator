using Sadie.Database.Models.Players;
using Sadie.Game.Players.Balance;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public interface IPlayerFactory
{
    IPlayerBalance CreateBalance(long credits,
        long pixels,
        long seasonal,
        long gotwPoints);
    
    PlayerLogic Create(
        INetworkObject networkObject, 
        PlayerData data,
        PlayerBalance balance);

    PlayerNavigatorSettings CreateNavigatorSettings(
        int windowX, 
        int windowY, 
        int windowWidth, 
        int windowHeight, 
        bool openSearches,
        int mode);

    PlayerGameSettings CreateSettings(int systemVolume,
        int furnitureVolume,
        int traxVolume,
        bool preferOldChat,
        bool blockRoomInvites,
        bool blockCameraFollow,
        int uiFlag,
        bool showNotifications);
}