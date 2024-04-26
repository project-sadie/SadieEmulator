using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomPaintWriter : AbstractPacketWriter
{
    public RoomPaintWriter(string type, string value)
    {
        WriteShort(ServerPacketId.RoomPaint);
        WriteString(type);
        WriteString(value);
    }
}