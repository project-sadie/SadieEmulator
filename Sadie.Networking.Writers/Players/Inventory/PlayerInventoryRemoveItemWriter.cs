using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryRemoveItem)]
public class PlayerInventoryRemoveItemWriter : AbstractPacketWriter
{
    public required long ItemId { get; init; }
}