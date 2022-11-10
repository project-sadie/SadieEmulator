using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserClosedWriter : NetworkPacketWriter
{
    public RoomUserClosedWriter()
    {
        WriteShort(ServerPacketId.RoomUserClosed);
    }
}