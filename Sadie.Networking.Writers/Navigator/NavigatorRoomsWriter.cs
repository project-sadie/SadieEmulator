using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorRoomsWriter : NetworkPacketWriter
{
    public NavigatorRoomsWriter(string tabName, string searchQuery)
    {
        WriteShort(ServerPacketId.NavigatorRooms);
        WriteString(tabName);
        WriteString(searchQuery);
        WriteInt(0); // amount
    }
}