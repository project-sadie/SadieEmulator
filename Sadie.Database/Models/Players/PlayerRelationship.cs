using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Players;

public class PlayerRelationship
{
    public int Id { get; set; }
    public int OriginPlayerId { get; set; }
    public int TargetPlayerId { get; set; }
    public PlayerRelationshipType Type { get; set; }
}