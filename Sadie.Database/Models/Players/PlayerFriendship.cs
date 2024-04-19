using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Players;

public class PlayerFriendship
{
    public int Id { get; init; }
    public int OriginPlayerId { get; init; }
    public Player OriginPlayer { get; init; }
    public int TargetPlayerId { get; init; }
    public Player TargetPlayer { get; init; }
    public PlayerFriendshipStatus Status { get; set; }
}