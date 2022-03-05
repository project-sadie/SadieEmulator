using Sadie.Game.Rooms;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorLiftedRoomsWriter : NetworkPacketWriter
{
    public NavigatorLiftedRoomsWriter(List<Room> rooms) : base(ServerPacketId.NavigatorLiftedRooms)
    {
        WriteInt(rooms.Count);

        foreach (var room in rooms)
        {
            WriteLong(room.Id);
            WriteInt(0); // unknown
            WriteString(""); // thumbnail?
            WriteString(room.Name);
        }
    }
}