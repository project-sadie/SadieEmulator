namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomPaint : NetworkPacketWriter
{
    internal RoomPaint(string type, string value) : base(ServerPacketId.RoomPaint)
    {
        WriteString(type);
        WriteString(value);
    }
}