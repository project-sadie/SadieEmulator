using Sadie.Game.Catalog.Items;
using Sadie.Game.Players.Inventory;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryAddItemsWriter : NetworkPacketWriter
{
    public PlayerInventoryAddItemsWriter(List<PlayerInventoryFurnitureItem> items)
    {
        WriteShort(ServerPacketId.PlayerInventoryAddItems);

        WriteInteger(1);
        WriteInteger(1);
        WriteInteger(items.Count);

        foreach (var item in items)
        {
            WriteLong(item.Id);
        }
    }
}