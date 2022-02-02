namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomPaintWriter : NetworkPacketWriter
{
    internal RoomPaintWriter(string type, string value) : base(ServerPacketId.RoomPaint)
    {
        WriteString(type);
        WriteString(value);
    }
}