using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets;

public class RoomUserClosedWriter : NetworkPacketWriter
{
    public RoomUserClosedWriter()
    {
        WriteShort(ServerPacketId.RoomUserClosed);
    }
}