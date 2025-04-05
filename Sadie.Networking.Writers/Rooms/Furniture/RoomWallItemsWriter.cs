using Sadie.API.Networking;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomWallItems)]
public class RoomWallItemsWriter : AbstractPacketWriter
{
    public required Dictionary<long, string> FurnitureOwners { get; init; }
    public required ICollection<PlayerFurnitureItemPlacementData> WallItems { get; init; }

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
                writer.WriteString(item.PlayerFurnitureItem.MetaData);
                writer.WriteInteger(-1);
                writer.WriteInteger(item.FurnitureItem.InteractionModes > 1 ? 1 : 0);
                writer.WriteLong(item.PlayerFurnitureItem.PlayerId);
            }
        });
    }
}