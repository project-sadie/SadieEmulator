using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomWallFurnitureItemPlaced)]
public class RoomWallFurnitureItemPlacedWriter : AbstractPacketWriter
{
    public required RoomFurnitureItem RoomFurnitureItem { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteString(RoomFurnitureItem.Id + "");
        writer.WriteInteger(RoomFurnitureItem.FurnitureItem.AssetId);
        writer.WriteString(RoomFurnitureItem.WallPosition);
        writer.WriteString(RoomFurnitureItem.MetaData);
        writer.WriteInteger(-1);
        writer.WriteInteger(RoomFurnitureItem.FurnitureItem.InteractionModes > 1 ? 1 : 0);
        writer.WriteLong(RoomFurnitureItem.OwnerId);
        writer.WriteString(RoomFurnitureItem.OwnerUsername);
    }
}