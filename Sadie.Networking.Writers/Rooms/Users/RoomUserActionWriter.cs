using Sadie.Game.Rooms.Users;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserActionWriter : NetworkPacketWriter
{
    public RoomUserActionWriter(int userId, int action)
    {
        WriteShort(ServerPacketId.RoomUserAction);
        WriteLong(userId);
        WriteLong(action);
    }
}