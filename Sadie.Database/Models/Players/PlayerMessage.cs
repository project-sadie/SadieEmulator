using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Database.Models.Players;

public class PlayerMessage
{
    public int Id { get; init; }
    [PacketData] public int OriginPlayerId { get; init; }
    public Player? OriginPlayer { get; init; }
    public int TargetPlayerId { get; init; }
    public Player? TargetPlayer { get; init; }
    [PacketData] public string? Message { get; init; }
    [PacketData] public DateTime CreatedAt { get; init; }
}