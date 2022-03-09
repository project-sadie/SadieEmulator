using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserDanceWriter : NetworkPacketWriter
{
    public RoomUserDanceWriter(long userId, int danceId)
    {
        WriteShort(ServerPacketId.RoomUserDance);
        WriteLong(userId);
        WriteLong(danceId);
    }
}