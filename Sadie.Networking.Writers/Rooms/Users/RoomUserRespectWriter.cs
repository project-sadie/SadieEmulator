using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserRespectWriter : AbstractPacketWriter
{
    public RoomUserRespectWriter(int userId, int totalRespects)
    {
        WriteShort(ServerPacketId.RoomUserRespect);
        WriteInteger(userId);
        WriteInteger(totalRespects);
    }
}