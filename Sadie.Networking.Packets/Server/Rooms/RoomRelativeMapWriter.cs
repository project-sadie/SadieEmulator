using Sadie.Game.Rooms;

namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomRelativeMapWriter : NetworkPacketWriter
{
    internal RoomRelativeMapWriter(RoomLayoutData layout) : base(ServerPacketId.RoomRelativeMap)
    {
        var mapSizeX = layout.SizeX;
        var mapSizeY = layout.SizeY;
        var mapSize = layout.SizeX * layout.SizeY;
        
        WriteInt(mapSize / mapSizeY);
        WriteInt(mapSize);

        for (var x = 0; x < mapSizeX * mapSizeY; x++)
        {
            WriteShort(16384);
        }
    }
}