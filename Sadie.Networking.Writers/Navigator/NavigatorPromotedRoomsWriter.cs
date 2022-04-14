using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorPromotedRoomsWriter : NetworkPacketWriter
{
    public NavigatorPromotedRoomsWriter()
    {
        WriteShort(ServerPacketId.NavigatorPromotedRooms);
        WriteInteger(2);
        WriteString("");
        WriteInteger(0);
        WriteBool(true);
        WriteInteger(0);
        WriteString("A");
        WriteString("B");
        WriteInteger(1);
        WriteString("C");
        WriteString("D");
        WriteInteger(1);
        WriteInteger(1);
        WriteInteger(1);
        WriteString("E");
    }
}