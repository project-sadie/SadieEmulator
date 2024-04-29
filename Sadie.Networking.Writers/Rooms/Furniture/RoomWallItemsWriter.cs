using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomWallItems)]
public class RoomWallItemsWriter : AbstractPacketWriter
{
    public required Dictionary<int, string> FurnitureOwners { get; init; }
    public required ICollection<RoomFurnitureItem> WallItems { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(WallItems))!, writer =>
        {
            writer.WriteInteger(WallItems.Count);

            foreach (var item in WallItems)
            {
                writer.WriteString(item.Id + "");
                writer.WriteInteger(item.FurnitureItem.AssetId);
                writer.WriteString(item.WallPosition);
                writer.WriteString(item.MetaData);
                writer.WriteInteger(-1);
                writer.WriteInteger(item.FurnitureItem.InteractionModes > 1 ? 1 : 0);
                writer.WriteLong(item.OwnerId);
            }
        });
    }
}