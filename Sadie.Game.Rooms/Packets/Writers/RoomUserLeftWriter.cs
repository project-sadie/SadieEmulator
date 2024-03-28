using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserLeftWriter : NetworkPacketWriter
{
    public RoomUserLeftWriter(int userId)
    {
        WriteShort(ServerPacketId.RoomUserLeft);
        WriteString(userId.ToString());
    }
}