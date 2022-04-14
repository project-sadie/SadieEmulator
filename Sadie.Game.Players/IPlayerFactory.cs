using Sadie.Database;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Components;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public interface IPlayerFactory
{
    IPlayerBalance CreateBalance(long credits,
        long pixels,
        long seasonal,
        long gotwPoints);
    
    IPlayer Create(
        INetworkObject networkObject, 
        IPlayerData data);

    PlayerNavigatorSettings CreateNavigatorSettings(int windowX, int windowY, int windowWidth, int windowHeight, bool openSearches, int unknown1);

    PlayerSettings CreateSettings(int systemVolume,
        int furnitureVolume,
        int traxVolume,
        bool preferOldChat,
        bool blockRoomInvites,
        bool blockCameraFollow,
        int uiFlag,
        bool showNotifications);
}