using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserActionWriter : NetworkPacketWriter
{
    public RoomUserActionWriter(int userId, int action)
    {
        WriteShort(ServerPacketId.RoomUserAction);
        WriteLong(userId);
        WriteLong(action);
    }
}