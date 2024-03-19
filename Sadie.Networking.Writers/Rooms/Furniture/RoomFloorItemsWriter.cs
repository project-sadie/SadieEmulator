using System.Collections;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

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
            WriteLong(item.Id);
            WriteInteger(item.FurnitureItem.AssetId);
            WriteInteger(item.Position.X);
            WriteInteger(item.Position.Y);
            WriteInteger((int) item.Direction);
            WriteString($"{item.Position.Z.ToString():0.00}");
            WriteString($"0"); // TODO: height

            switch (item.FurnitureItem.InteractionType)
            {
                default:
                    WriteInteger(1); 
                    WriteInteger(0); 
                    WriteString(item.MetaData);
                    break;
            }
            
            WriteInteger(-1); // TODO: check
            WriteInteger(item.FurnitureItem.InteractionModes > 1 ? 1 : 0); 
            WriteLong(item.OwnerId);
        }
    }
}