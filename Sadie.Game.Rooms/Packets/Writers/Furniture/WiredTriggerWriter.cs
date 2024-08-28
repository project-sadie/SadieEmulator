using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Furniture;

[PacketId(ServerPacketId.WiredTrigger)]
public class WiredTriggerWriter : AbstractPacketWriter
{
    public required bool StuffTypeSelectionEnabled { get; init; }
    public required int MaxItemsSelected { get; init; }
    public required List<int> SelectedItemIds { get; init; }
    public required int AssetId { get; init; }
    public required int Id { get; init; }
    public required string Input { get; init; }
    public required int Unknown1 { get; init; }
    public required int Unknown2 { get; init; }
    public required int TriggerCode { get; init; }
    public required int Unknown3 { get; init; }
    public required int Unknown4 { get; init; }
}