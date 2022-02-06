using Sadie.Game.Rooms;

namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorLiftedRoomsWriter : NetworkPacketWriter
{
    internal NavigatorLiftedRoomsWriter(List<RoomEntity> rooms) : base(ServerPacketId.NavigatorLiftedRooms)
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