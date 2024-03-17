using Sadie.Game.Furniture;
using Sadie.Game.Players.Inventory;
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
        WriteInteger(furnitureItem.Id);
        WriteString(furnitureItem.Type.ToString());
        WriteLong(item.Id);
        WriteInteger(furnitureItem.AssetId);

        var names = new List<string>
        {
            "floor", "landscape", "wallpaper", "poster"
        };

        if (names.Contains(furnitureItem.Name))
        {
            switch (furnitureItem.Name)
            {
                case "landscape":
                    WriteInteger(4);
                    break;
                case "floor":
                    WriteInteger(3);
                    break;
                case "wallpaper":
                    WriteInteger(2);
                    break;
                case "poster":
                    WriteInteger(6);
                    break;
            }

            WriteInteger(0);
            WriteString(item.MetaData);
        }
        else
        {
            WriteInteger(furnitureItem.Name == "gnome_box" ? 13 : 1);
        }

        WriteBool(furnitureItem.CanRecycle);
        WriteBool(furnitureItem.CanTrade);
        WriteBool(furnitureItem.CanInventoryStack);
        WriteBool(false);
        WriteInteger(-1);
        WriteBool(true);
        WriteInteger(-1);

        if (furnitureItem.Type != FurnitureItemType.Floor)
        {
            return;
        }
        
        WriteString(string.Empty);
        WriteInteger(0);
    }
}