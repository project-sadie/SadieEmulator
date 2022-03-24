using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserClosedWriter : NetworkPacketWriter
{
    public RoomUserClosedWriter()
    {
        WriteShort(ServerPacketId.RoomUserClosed);
    }
}