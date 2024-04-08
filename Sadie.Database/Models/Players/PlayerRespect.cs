namespace Sadie.Database.Models.Players;

public class PlayerRespect
{
    public int Id { get; set; }
    public int OriginPlayerId { get; set; }
    public int TargetPlayerId { get; set; }
}