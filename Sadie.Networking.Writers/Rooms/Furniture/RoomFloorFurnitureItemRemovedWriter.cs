using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomFloorFurnitureItemRemovedWriter : NetworkPacketWriter
{
    public RoomFloorFurnitureItemRemovedWriter(long id, bool expired, long ownerId, int delay)
    {
        WriteShort(ServerPacketId.RoomFurnitureItemRemoved);
        WriteString(id.ToString());
        WriteBool(expired);
        WriteLong(ownerId);
        WriteInteger(delay);
    }
}