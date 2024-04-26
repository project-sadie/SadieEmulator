using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserClosedWriter : AbstractPacketWriter
{
    public RoomUserClosedWriter()
    {
        WriteShort(ServerPacketId.RoomUserClosed);
    }
}