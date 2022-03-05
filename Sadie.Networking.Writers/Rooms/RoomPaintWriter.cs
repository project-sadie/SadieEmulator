using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomPaintWriter : NetworkPacketWriter
{
    public RoomPaintWriter(string type, string value)
    {
        WriteShort(ServerPacketId.RoomPaint);
        WriteString(type);
        WriteString(value);
    }
}