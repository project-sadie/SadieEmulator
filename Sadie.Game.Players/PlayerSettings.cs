namespace Sadie.Game.Players;

public class PlayerSettings(
    int systemVolume,
    int furnitureVolume,
    int traxVolume,
    bool oldChat,
    bool blockRoomInvites,
    bool blockCameraFollow,
    int uiFlags,
    bool showNotifications)
{
    public int SystemVolume { get; } = systemVolume;
    public int FurnitureVolume { get; } = furnitureVolume;
    public int TraxVolume { get; } = traxVolume;
    public bool OldChat { get; } = oldChat;
    public bool BlockRoomInvites { get; } = blockRoomInvites;
    public bool BlockCameraFollow { get; } = blockCameraFollow;
    public int UiFlags { get; } = uiFlags;
    public bool ShowNotifications { get; } = showNotifications;
}