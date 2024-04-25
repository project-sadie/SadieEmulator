using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomRelativeMapWriter : NetworkPacketWriter
{
    public RoomRelativeMapWriter(RoomTileMap tileMap, ICollection<RoomFurnitureItem> items)
    {
        WriteShort(ServerPacketId.RoomRelativeMap);
        WriteInteger(tileMap.Size / tileMap.SizeY);
        WriteInteger(tileMap.Size);

        for (var y = 0; y < tileMap.SizeY; y++)
        {
            for (var x = 0; x < tileMap.SizeX; x++)
            {
                var topItem = RoomTileMapHelpers
                    .GetItemsForPosition(x, y, items)
                    .MaxBy(i => i.PositionZ);
                
                WriteShort((short)(topItem?.PositionZ ?? 0));
            }
        }
    }
}