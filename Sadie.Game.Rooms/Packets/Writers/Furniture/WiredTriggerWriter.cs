using Sadie.API.Networking;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Furniture;

[PacketId(ServerPacketId.WiredTrigger)]
public class WiredTriggerWriter : AbstractPacketWriter
{
    public required bool StuffTypeSelectionEnabled { get; init; }
    public required int MaxItemsSelected { get; init; }
    public required List<int> SelectedItemIds { get; init; }
    public required int StuffTypeId { get; init; }
    public required int Id { get; init; }
    public string Input { get; init; } = "";
    public required List<int> IntParameters { get; init; }
    public required int StuffTypeSelectionCode { get; init; }
    public required int TriggerConfig { get; init; }
    public required List<int> ConflictingEffectIds { get; init; }

    public override void OnConfigureRules()
    {
        if (TriggerConfig == (int) WiredTriggerCode.AvatarSaysSomething && 
            IntParameters[0] == 0)
        {
            Override(GetType().GetProperty(nameof(IntParameters))!, writer =>
            { 
                writer.WriteInteger(0);
            });
        }
    }
}