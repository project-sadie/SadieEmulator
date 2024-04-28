using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomRelativeMap)]
public class RoomRelativeMapWriter : AbstractPacketWriter
{
    public RoomTileMap TileMap { get; }
    public ICollection<RoomFurnitureItem> Items { get; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(TileMap.Size / TileMap.SizeY);
        writer.WriteInteger(TileMap.Size);

        for (var y = 0; y < TileMap.SizeY; y++)
        {
            for (var x = 0; x < TileMap.SizeX; x++)
            {
                var topItem = RoomTileMapHelpers
                    .GetItemsForPosition(x, y, Items)
                    .MaxBy(i => i.PositionZ);
                
                writer.WriteShort((short)(topItem?.PositionZ ?? 0));
            }
        }
    }
}