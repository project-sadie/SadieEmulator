using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Players.Packets;

public class PlayerInventoryAddItemsWriter : NetworkPacketWriter
{
    public PlayerInventoryAddItemsWriter(List<PlayerFurnitureItem> items)
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