using Sadie.Database.Models.Players;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryFurnitureItemsWriter : AbstractPacketWriter
{
    public PlayerInventoryFurnitureItemsWriter(
        int pages, 
        int currentPage, 
        List<PlayerFurnitureItem> items)
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
    
    private void WriteItem(PlayerFurnitureItem item)
    {
        var furnitureItem = item.FurnitureItem;
        var hasRentPeriodStarted = false;
        var slotId = "";
        var extra = 1;
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