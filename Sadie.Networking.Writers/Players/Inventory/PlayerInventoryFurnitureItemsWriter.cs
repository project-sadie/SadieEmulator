using Sadie.Game.Furniture;
using Sadie.Game.Players.Inventory;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

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
        var furnitureItem = item.FurnitureItem;
        var reference = furnitureItem.Id;
        var hasRentPeriodStarted = false;
        var flatId = furnitureItem.Id;
        var slotId = "";
        var extra = 1;
        var objectData = new Dictionary<string, string>();
        var expiresInSeconds = -1;
        
        WriteLong(item.Id);
        WriteString(EnumHelpers.GetEnumDescription(furnitureItem.Type).ToUpper());
        WriteLong(item.Id);
        WriteInteger(furnitureItem.AssetId);
        
        switch (furnitureItem.AssetName)
        {
            case "floor":
                WriteInteger(3);
                WriteInteger(0);
                WriteString(item.MetaData);
                break;
            case "wallpaper":
                WriteInteger(2);
                WriteInteger(0);
                WriteString(item.MetaData);
                break;
            case "landscape":
                WriteInteger(4);
                WriteInteger(0);
                WriteString(item.MetaData);
                break;
            default:
                WriteInteger(1);
                WriteInteger(1);
                WriteInteger(0);
                break;
        }
        
        WriteBool(furnitureItem.CanRecycle);
        WriteBool(furnitureItem.CanTrade);
        WriteBool(furnitureItem.CanInventoryStack);
        WriteBool(furnitureItem.CanMarketplaceSell);
        WriteInteger(expiresInSeconds);
        WriteBool(hasRentPeriodStarted);
        WriteInteger(-1);

        if (furnitureItem.Type != FurnitureItemType.Floor)
        {
            return;
        }
        
        WriteString(slotId);
        WriteInteger(extra);
    }
}