using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Furniture;

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