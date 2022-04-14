using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserDanceWriter : NetworkPacketWriter
{
    public RoomUserDanceWriter(int userId, int danceId)
    {
        WriteShort(ServerPacketId.RoomUserDance);
        WriteInteger(userId);
        WriteInteger(danceId);
    }
}