using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

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