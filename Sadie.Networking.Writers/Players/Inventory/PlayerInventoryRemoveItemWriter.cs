using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryRemoveItem)]
public class PlayerInventoryRemoveItemWriter : AbstractPacketWriter
{
    public required long ItemId { get; init; }
}