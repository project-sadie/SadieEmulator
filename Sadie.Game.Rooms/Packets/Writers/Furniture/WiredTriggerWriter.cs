using Sadie.API.Networking;
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
    public string Input { get; init; } = "";
    public required List<int> IntParameters { get; init; }
    public required int StuffTypeSelectionCode { get; init; }
    public required int TriggerConfig { get; init; }
    public required List<int> ConflictingEffectIds { get; init; }
}