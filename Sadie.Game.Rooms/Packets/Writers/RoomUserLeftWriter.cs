using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserLeftWriter : NetworkPacketWriter
{
    public RoomUserLeftWriter(int userId)
    {
        WriteShort(ServerPacketId.RoomUserLeft);
        WriteString(userId.ToString());
    }
}