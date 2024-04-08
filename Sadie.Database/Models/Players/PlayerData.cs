namespace Sadie.Database.Models.Players;

public class PlayerData
{
    public int Id { get; set; }
    public Player Player { get; set; }
    public int PlayerId { get; set; }
    public int HomeRoomId { get; set; }
    public int CreditBalance { get; set; }
    public int PixelBalance { get; set; }
    public int SeasonalBalance { get; set; }
    public int GotwPoints { get; set; }
    public int RespectPoints { get; set; }
    public int RespectPointsPet { get; set; }
    public int AchievementScore { get; set; }
    public bool AllowFriendRequests { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastOnline { get; set; }
}