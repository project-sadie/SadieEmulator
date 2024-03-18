using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryRemoveItemWriter : NetworkPacketWriter
{
    public PlayerInventoryRemoveItemWriter(long itemId)
    {
        WriteShort(ServerPacketId.PlayerInventoryRemoveItem);
        WriteLong(itemId);
    }
}