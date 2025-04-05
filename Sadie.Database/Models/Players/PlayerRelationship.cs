using Sadie.Enums.Game.Players;

namespace Sadie.Database.Models.Players;

public class PlayerRelationship
{
    public int Id { get; init; }
    public long OriginPlayerId { get; init; }
    public Player? OriginPlayer { get; init; }
    public long TargetPlayerId { get; init; }
    public Player? TargetPlayer { get; init; }
    public PlayerRelationshipType TypeId { get; set; }
}