using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomFurnitureItemRemovedWriter : NetworkPacketWriter
{
    public RoomFurnitureItemRemovedWriter(RoomFurnitureItem roomFurnitureItem)
    {
        WriteShort(ServerPacketId.RoomFurnitureItemRemoved);
        WriteString(roomFurnitureItem.Id.ToString());
        WriteBool(false); // TODO: check
        WriteLong(roomFurnitureItem.OwnerId);
        WriteInteger(0); // TODO: check
    }
}