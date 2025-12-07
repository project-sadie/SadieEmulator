using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryRemoveItem)]
public class PlayerInventoryRemoveItemWriter : AbstractPacketWriter
{
    public required long ItemId { get; init; }
}