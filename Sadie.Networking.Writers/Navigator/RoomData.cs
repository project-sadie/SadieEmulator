using Sadie.Shared.Attributes;

namespace Sadie.Networking.Writers.Navigator;

public class RoomData
{
    [PacketData] public required int Id { get; set; }
    [PacketData] public required string Name { get; set; }
    [PacketData] public required long OwnerId { get; set; }
    [PacketData] public required string OwnerUsername { get; set; }
    [PacketData] public required int AccessType { get; set; }
    [PacketData] public required int UserCount { get; set; }
    [PacketData] public required int MaxUsers { get; set; }
    [PacketData] public required string Description { get; set; }
    [PacketData] public required int TradeOption { get; set; }
    [PacketData] public required int Score { get; set; }
    [PacketData] public required int Ranking { get; set; }
    [PacketData] public required int CategoryId { get; set; }
    [PacketData] public required List<string> Tags { get; set; }
    [PacketData] public required int Bitmask { get; set; }
}