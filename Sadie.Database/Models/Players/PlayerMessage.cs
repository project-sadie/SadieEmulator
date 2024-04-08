namespace Sadie.Database.Models.Players;

public class PlayerMessage
{
    public int OriginPlayerId { get; set; }
    public Player OriginPlayer { get; set; }
    public int TargetPlayerId { get; set; }
    public Player TargetPlayer { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}