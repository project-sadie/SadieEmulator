using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomWallFurnitureItemRemovedWriter : AbstractPacketWriter
{
    public RoomWallFurnitureItemRemovedWriter(RoomFurnitureItem item)
    {
        WriteShort(ServerPacketId.RoomWallFurnitureItemRemoved);
        WriteString(item.Id.ToString());
        WriteLong(item.OwnerId);
    }
}