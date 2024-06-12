using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomFloorItems)]
public class RoomFloorItemsWriter : AbstractPacketWriter
{
    public required Dictionary<int, string?> FurnitureOwners { get; init; }
    public required ICollection<RoomFurnitureItem> FloorItems { get; init; }

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
        writer.WriteInteger((int) ObjectDataKey.LegacyKey); 
        writer.WriteString(item.MetaData);
        writer.WriteInteger(-1);
        writer.WriteInteger(item.FurnitureItem.InteractionModes > 1 ? 1 : 0); 
        writer.WriteLong(item.OwnerId);
    }
}