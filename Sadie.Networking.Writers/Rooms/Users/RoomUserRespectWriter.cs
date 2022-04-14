using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserRespectWriter : NetworkPacketWriter
{
    public RoomUserRespectWriter(int userId, int totalRespects)
    {
        WriteShort(ServerPacketId.RoomUserRespect);
        WriteInteger(userId);
        WriteInteger(totalRespects);
    }
}