using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomWallFurnitureItemPlacedWriter : AbstractPacketWriter
{
    public RoomWallFurnitureItemPlacedWriter(RoomFurnitureItem roomFurnitureItem)
    {
        WriteShort(ServerPacketId.RoomWallFurnitureItemPlaced);
        WriteString(roomFurnitureItem.Id + "");
        WriteInteger(roomFurnitureItem.FurnitureItem.AssetId);
        WriteString(roomFurnitureItem.WallPosition);
        WriteString(roomFurnitureItem.MetaData);
        WriteInteger(-1);
        WriteInteger(roomFurnitureItem.FurnitureItem.InteractionModes > 1 ? 1 : 0);
        WriteLong(roomFurnitureItem.OwnerId);
        WriteString(roomFurnitureItem.OwnerUsername);
    }
}