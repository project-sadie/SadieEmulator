namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorPromotedRoomsWriter : NetworkPacketWriter
{
    public NavigatorPromotedRoomsWriter() : base(ServerPacketId.NavigatorPromotedRooms)
    {
        // TODO: Pass structure in 
        
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