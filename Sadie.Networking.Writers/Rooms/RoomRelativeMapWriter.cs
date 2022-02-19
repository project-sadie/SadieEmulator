using Sadie.Game.Rooms;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomRelativeMapWriter : NetworkPacketWriter
{
    public RoomRelativeMapWriter(RoomLayoutData layout) : base(ServerPacketId.RoomRelativeMap)
    {
        WriteInt(layout.Size / layout.SizeY);
        WriteInt(layout.Size);

        for (var y = 0; y < layout.SizeY; y++)
        {
            for (var x = 0; x < layout.SizeX; x++)
            {
                var tile = layout.GetTile(x, y);
                var height = tile?.Point.Z * 256.0 ?? 0;

                if (tile is {State: RoomTileState.Closed})
                {
                    height = short.MaxValue;
                }
                
                WriteShort((short)height);
            }
        }
    }
}