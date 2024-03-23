using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomWallFurnitureItemRemovedWriter : NetworkPacketWriter
{
    public RoomWallFurnitureItemRemovedWriter(RoomFurnitureItem item)
    {
        WriteShort(ServerPacketId.RoomWallFurnitureItemRemoved);
        WriteString(item.Id.ToString());
        WriteLong(item.OwnerId);
    }
}