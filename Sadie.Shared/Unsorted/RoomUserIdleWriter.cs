using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Shared.Unsorted;

public class RoomUserIdleWriter : NetworkPacketWriter
{
    public RoomUserIdleWriter(int userId, bool isIdle)
    {
        WriteShort(ServerPacketId.RoomUserIdle);
        WriteInteger(userId);
        WriteBool(isIdle);
    }
}