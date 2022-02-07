namespace Sadie.Game.Players;

public class PlayerSettings
{
    public int SystemVolume { get; }
    public int FurnitureVolume { get; }
    public int TraxVolume { get; }
    public bool OldChat { get; }
    public bool BlockRoomInvites { get; }
    public bool BlockCameraFollow { get; }
    public int UiFlags { get; }
    public int ChatColor { get; }
    
    public PlayerSettings(int systemVolume, int furnitureVolume, int traxVolume, bool oldChat, bool blockRoomInvites, bool blockCameraFollow, int uiFlags, int chatColor)
    {
        SystemVolume = systemVolume;
        FurnitureVolume = furnitureVolume;
        TraxVolume = traxVolume;
        OldChat = oldChat;
        BlockRoomInvites = blockRoomInvites;
        BlockCameraFollow = blockCameraFollow;
        UiFlags = uiFlags;
        ChatColor = chatColor;
    }
}