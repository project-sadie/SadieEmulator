using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomFloorItemsWriter : NetworkPacketWriter
{
    public RoomFloorItemsWriter(ICollection<RoomFurnitureItem> floorItems,
        Dictionary<long, string> furnitureOwners)
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
            var height = -1; // TODO: height
            var extra = 1;
            
            WriteLong(item.Id);
            WriteInteger(item.FurnitureItem.AssetId);
            WriteInteger(item.Position.X);
            WriteInteger(item.Position.Y);
            WriteInteger((int) item.Direction);
            WriteString($"{item.Position.Z.ToString():0.00}");
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

            var expires = -1; // TODO: Add to item
            
            WriteString(item.MetaData);
            WriteInteger(expires);
            WriteInteger(item.FurnitureItem.InteractionModes > 1 ? 1 : 0); 
            WriteLong(item.OwnerId);
        }
    }
}