namespace Sadie.Networking.Packets.Server.Rooms;

public class RoomPaintWriter : NetworkPacketWriter
{
    public RoomPaintWriter(string type, string value) : base(ServerPacketId.RoomPaint)
    {
        WriteString(type);
        WriteString(value);
    }
}