using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomFloorFurnitureItemRemovedWriter : NetworkPacketWriter
{
    public RoomFloorFurnitureItemRemovedWriter(RoomFurnitureItem item)
    {
        WriteShort(ServerPacketId.RoomFurnitureItemRemoved);
        WriteString(item.Id.ToString());
        WriteBool(false); // TODO: check
        WriteLong(item.OwnerId);
        WriteInteger(0); // TODO: check
    }
}