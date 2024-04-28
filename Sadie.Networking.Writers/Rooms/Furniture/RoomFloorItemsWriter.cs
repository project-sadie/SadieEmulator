using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomFloorItems)]
public class RoomFloorItemsWriter : AbstractPacketWriter
{
    public required ICollection<RoomFurnitureItem> FloorItems { get; init; }
    public required Dictionary<int, string> FurnitureOwners { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(FurnitureOwners.Count);

        foreach (var owner in FurnitureOwners)
        {
           writer.WriteLong(owner.Key);
           writer.WriteString(owner.Value);
        }
        
        writer.WriteInteger(FloorItems.Count);

        foreach (var item in FloorItems)
        {
            WriteItem(item, writer);
        }
    }

    private void WriteItem(RoomFurnitureItem item, NetworkPacketWriter writer)
    {
        var height = -1; // TODO: height
        var extra = 1;
            
        writer.WriteLong(item.Id);
        writer.WriteInteger(item.FurnitureItem.AssetId);
        writer.WriteInteger(item.PositionX);
        writer.WriteInteger(item.PositionY);
        writer.WriteInteger((int) item.Direction);
        writer.WriteString($"{item.PositionZ.ToString():0.00}");
        writer.WriteString(height.ToString());
        writer.WriteInteger(extra);
        
        var objectData = new Dictionary<string, string>();
            
        switch (item.FurnitureItem.InteractionType)
        {
            default:
                writer.WriteInteger((int) ObjectDataKey.MapKey); 
                writer.WriteInteger(objectData.Count);

                foreach (var dataPair in objectData)
                {
                    writer.WriteString(dataPair.Key);
                    writer.WriteString(dataPair.Value);
                }
                break;
        }
        
        writer.WriteInteger(-1);
        writer.WriteInteger(item.FurnitureItem.InteractionModes > 1 ? 1 : 0); 
        writer.WriteLong(item.OwnerId);
    }
}