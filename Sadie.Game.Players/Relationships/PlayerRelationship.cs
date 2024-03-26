using Sadie.Game.Players.Friendships;

namespace Sadie.Game.Players.Relationships;

public class PlayerRelationship(int id, long originPlayerId, long targetPlayerId, PlayerRelationshipType type)
{
    public int Id { get; } = id;
    public long OriginPlayerId { get; } = originPlayerId;
    public long TargetPlayerId { get; } = targetPlayerId;
    public PlayerRelationshipType Type { get; set; } = type;
}