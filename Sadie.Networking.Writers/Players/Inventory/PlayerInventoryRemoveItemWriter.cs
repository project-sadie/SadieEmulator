using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryRemoveItemWriter : AbstractPacketWriter
{
    public PlayerInventoryRemoveItemWriter(long itemId)
    {
        WriteShort(ServerPacketId.PlayerInventoryRemoveItem);
        WriteLong(itemId);
    }
}