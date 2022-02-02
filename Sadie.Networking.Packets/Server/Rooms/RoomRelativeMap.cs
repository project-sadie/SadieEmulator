using Sadie.Shared;

namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomRelativeMap : NetworkPacketWriter
{
    internal RoomRelativeMap() : base(ServerPacketId.RoomRelativeMap)
    {
        var model = SadieConstants.MockHeightmap.Split("\n");
        var mapSizeX = model[0].Length;
        var mapSizeY = model.Length;
        var mapSize = mapSizeX * mapSizeY;
        
        WriteInt(mapSize / mapSizeY);
        WriteInt(mapSize);

        for (var y = 0; y < mapSizeY; y++)
        {
            for (var x = 0; x < mapSizeX; x++)
            {
                WriteShort(64 * 256);
            }
        }
    }
}