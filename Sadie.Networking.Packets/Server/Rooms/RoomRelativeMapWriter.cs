namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomRelativeMapWriter : NetworkPacketWriter
{
    internal RoomRelativeMapWriter(string heightMap) : base(ServerPacketId.RoomRelativeMap)
    {
        var heightMapLines = heightMap.Split("\n");
        var mapSizeX = heightMapLines.OrderByDescending(x => x.Length).First().Length;
        var mapSizeY = heightMapLines.Length;
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