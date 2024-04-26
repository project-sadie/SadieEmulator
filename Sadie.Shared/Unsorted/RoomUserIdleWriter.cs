using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Shared.Unsorted;

public class RoomUserIdleWriter : AbstractPacketWriter
{
    public RoomUserIdleWriter(int userId, bool isIdle)
    {
        WriteShort(ServerPacketId.RoomUserIdle);
        WriteInteger(userId);
        WriteBool(isIdle);
    }
}