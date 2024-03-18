using Sadie.Game.Furniture;
using Sadie.Game.Players.Inventory;
using Sadie.Shared.Helpers;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryFurnitureItemsWriter : NetworkPacketWriter
{
    public PlayerInventoryFurnitureItemsWriter(
        int pages, 
        int currentPage, 
        List<PlayerInventoryFurnitureItem> items)
    {
        WriteShort(ServerPacketId.PlayerInventoryFurnitureItems);
        
        WriteInteger(pages);
        WriteInteger(currentPage);
        WriteInteger(items.Count);

        foreach (var item in items)
        {
            WriteItem(item);
        }
    }
    
    private void WriteItem(PlayerInventoryFurnitureItem item)
    {
        var furnitureItem = item.Item;
        WriteLong(item.Id);
        WriteString(EnumHelpers.GetEnumDescription(furnitureItem.Type).ToUpper()); // has to be upper or fails
        WriteLong(item.Id);
        WriteInteger(furnitureItem.AssetId);

        WriteInteger(1);
        WriteInteger(0);
        WriteString(item.MetaData);
        
        WriteBool(furnitureItem.CanRecycle);
        WriteBool(furnitureItem.CanTrade);
        WriteBool(furnitureItem.CanInventoryStack);
        WriteBool(furnitureItem.CanMarketplaceSell);
        WriteInteger(-1);
        WriteBool(true);
        WriteInteger(-1);

        if (furnitureItem.Type != FurnitureItemType.Floor)
        {
            return;
        }
        
        WriteString(string.Empty);
        WriteInteger(1);
    }
}