using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomWallItemsWriter : NetworkPacketWriter
{
    public RoomWallItemsWriter(ICollection<RoomFurnitureItem> wallItems)
    {
       WriteShort(ServerPacketId.RoomWallItems);
        
       WriteInteger(0);
       WriteInteger(0);
    }
}