using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.WiredEffect)]
public class WiredMessageEffectWriter : AbstractPacketWriter
{
    public required bool StuffTypeSelectionEnabled { get; init; }
    public required int MaxItemsSelected { get; init; }
    public required List<int> SelectedItemIds { get; init; }
    public required int WiredEffectType { get; init; }
    public required int Id { get; init; }
    public required string Input { get; init; }
    public required List<int> IntParams { get; init; }
    public required int StuffTypeSelectionCode { get; init; }
    public required int Type { get; init; }
    public required int DelayInPulses { get; init; }
    public required List<int> ConflictingTriggerIds { get; init; }
}