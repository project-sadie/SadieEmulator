namespace Sadie.Database.Models.Players;

public class PlayerGameSettings
{
    public int Id { get; set; }
    public long PlayerId { get; set; }
    public Player? Player { get; set; }
    public int SystemVolume { get; init; }
    public int FurnitureVolume { get; init; }
    public int TraxVolume { get; init; }
    public bool PreferOldChat { get; init; }
    public bool BlockRoomInvites { get; init; }
    public bool BlockCameraFollow { get; init; }
    public int UiFlags { get; init; }
    public bool ShowNotifications { get; init; }
} 