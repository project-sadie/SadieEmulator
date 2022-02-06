namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorPromotedRoomsWriter : NetworkPacketWriter
{
    internal NavigatorPromotedRoomsWriter() : base(ServerPacketId.NavigatorPromotedRooms)
    {
        WriteInt(2);
        WriteString("");
        WriteInt(0);
        WriteBoolean(true);
        WriteInt(0);
        WriteString("A");
        WriteString("B");
        WriteInt(1);
        WriteString("C");
        WriteString("D");
        WriteInt(1);
        WriteInt(1);
        WriteInt(1);
        WriteString("E");
    }
}