namespace Sadie.Database.Models.Players;

public class PlayerRespect
{
    public int Id { get; set; }
    public int OriginPlayerId { get; set; }
    public Player OriginPlayer { get; set; }
    public int TargetPlayerId { get; set; }
    public Player TargetPlayer { get; set; }
}