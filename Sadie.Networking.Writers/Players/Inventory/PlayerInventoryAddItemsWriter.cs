using Sadie.Game.Players.Inventory;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryAddItemsWriter : NetworkPacketWriter
{
    public PlayerInventoryAddItemsWriter(List<PlayerInventoryFurnitureItem> items)
    {
        var category = 1;
        
        WriteShort(ServerPacketId.PlayerInventoryAddItems);
        WriteInteger(items.Count);
        WriteInteger(category);
        WriteInteger(items.Count);

        foreach (var item in items)
        {
            WriteLong(item.Id);
        }
    }
}