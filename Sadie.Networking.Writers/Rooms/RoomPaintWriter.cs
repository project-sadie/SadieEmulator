using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomPaintWriter : NetworkPacketWriter
{
    public RoomPaintWriter(string type, string value) : base(ServerPacketId.RoomPaint)
    {
        WriteString(type);
        WriteString(value);
    }
}