using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomFloorItemsWriter : AbstractPacketWriter
{
    public RoomFloorItemsWriter(
        ICollection<RoomFurnitureItem> floorItems, 
        Dictionary<int, string> furnitureOwners)
    {
        
        WriteShort(ServerPacketId.RoomFloorItems);
        
        WriteInteger(furnitureOwners.Count);

        foreach (var owner in furnitureOwners)
        {
           WriteLong(owner.Key);
           WriteString(owner.Value);
        }
        
        WriteInteger(floorItems.Count);

        foreach (var item in floorItems)
        {
            WriteItem(item);
        }
    }

    private void WriteItem(RoomFurnitureItem item)
    {
        var height = -1; // TODO: height
        var extra = 1;
            
        WriteLong(item.Id);
        WriteInteger(item.FurnitureItem.AssetId);
        WriteInteger(item.PositionX);
        WriteInteger(item.PositionY);
        WriteInteger((int) item.Direction);
        WriteString($"{item.PositionZ.ToString():0.00}");
        WriteString(height.ToString());
        WriteInteger(extra);
        
        var objectData = new Dictionary<string, string>();
            
        switch (item.FurnitureItem.InteractionType)
        {
            default:
                WriteInteger((int) ObjectDataKey.MapKey); 
                WriteInteger(objectData.Count);

                foreach (var dataPair in objectData)
                {
                    WriteString(dataPair.Key);
                    WriteString(dataPair.Value);
                }
                break;
        }
        
        WriteInteger(-1);
        WriteInteger(item.FurnitureItem.InteractionModes > 1 ? 1 : 0); 
        WriteLong(item.OwnerId);
    }
}