namespace Sadie.Database.Models.Players;

public class PlayerFriendship
{
    public int Id { get; }
    public int OriginPlayerId { get; }
    public int TargetPlayerId { get; }
    public PlayerFriendshipStatus Status { get; set; }
}