using Sadie.Game.Rooms.Users;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserIdleWriter : NetworkPacketWriter
{
    public RoomUserIdleWriter(IRoomUser user)
    {
        WriteShort(ServerPacketId.RoomUserIdle);
        WriteInteger(user.Id);
        WriteBool(user.IsIdle);
    }
}