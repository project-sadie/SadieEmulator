using Sadie.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomRelativeMapWriter : NetworkPacketWriter
{
    public RoomRelativeMapWriter(RoomTileMap layout)
    {
        WriteShort(ServerPacketId.RoomRelativeMap);
        WriteInteger(layout.Size / layout.SizeY);
        WriteInteger(layout.Size);

        for (var y = 0; y < layout.SizeY; y++)
        {
            for (var x = 0; x < layout.SizeX; x++)
            {
                var tile = layout.FindTile(x, y);

                if (tile == null)
                {
                    WriteShort(short.MaxValue);
                }
                else
                {
                    WriteShort((short)tile.Point.Z);
                }
            }
        }
    }
}