using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets;

public class RoomUserLeftWriter : NetworkPacketWriter
{
    public RoomUserLeftWriter(long userId)
    {
        WriteShort(ServerPacketId.RoomUserLeft);
        WriteString(userId.ToString());
    }
}