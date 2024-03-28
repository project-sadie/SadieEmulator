using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

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