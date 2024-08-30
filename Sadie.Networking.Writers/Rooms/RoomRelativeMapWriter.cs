using Sadie.API;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomRelativeMap)]
public class RoomRelativeMapWriter : AbstractPacketWriter
{
    public required IRoomTileMap TileMap { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(TileMap.Size / TileMap.SizeY);
        writer.WriteInteger(TileMap.Size);

        for (var y = 0; y < TileMap.SizeY; y++)
        {
            for (var x = 0; x < TileMap.SizeX; x++)
            {
                var height = TileMap.TileExistenceMap[y, x] == 1 ? 
                    TileMap.ZMap[y, x] *  256.0 : 
                    short.MaxValue;
                
                writer.WriteShort((short) height);
            }
        }
    }
}