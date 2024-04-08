using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Players;

public class PlayerFriendship
{
    public int Id { get; }
    public int OriginPlayerId { get; set; }
    public Player OriginPlayer { get; }
    public int TargetPlayerId { get; set; }
    public Player TargetPlayer { get; }
    public PlayerFriendshipStatus Status { get; set; }
}