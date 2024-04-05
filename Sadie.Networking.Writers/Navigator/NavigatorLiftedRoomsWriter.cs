using Sadie.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorLiftedRoomsWriter : NetworkPacketWriter
{
    public NavigatorLiftedRoomsWriter(List<Room> rooms)
    {
        WriteShort(ServerPacketId.NavigatorLiftedRooms);
        WriteInteger(rooms.Count);

        foreach (var room in rooms)
        {
            WriteLong(room.Id);
            WriteInteger(0); // unknown
            WriteString(""); // thumbnail?
            WriteString(room.Name);
        }
    }
}