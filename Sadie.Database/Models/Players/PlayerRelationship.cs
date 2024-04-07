namespace Sadie.Database.Models.Players;

public class PlayerRelationship
{
    public int OriginPlayerId { get; }
    public int TargetPlayerId { get; }
    public int TypeId { get; }
}