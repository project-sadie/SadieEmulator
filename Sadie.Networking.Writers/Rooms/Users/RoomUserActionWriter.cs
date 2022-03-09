using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserActionWriter : NetworkPacketWriter
{
    public RoomUserActionWriter(long userId, int actionId)
    {
        WriteShort(ServerPacketId.RoomUserAction);
        WriteLong(userId);
        WriteLong(actionId);
    }
}