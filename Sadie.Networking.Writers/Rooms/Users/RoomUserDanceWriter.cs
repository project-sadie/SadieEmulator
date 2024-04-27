using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserDanceWriter : AbstractPacketWriter
{
    public RoomUserDanceWriter(int userId, int danceId)
    {
        WriteShort(ServerPacketId.RoomUserDance);
        WriteInteger(userId);
        WriteInteger(danceId);
    }
}