using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Rights;

public class RoomGiveUserRightsWriter : NetworkPacketWriter
{
    public RoomGiveUserRightsWriter(int roomId, int playerId, string playerUsername)
    {
        WriteShort(ServerPacketId.RoomGiveUserRights);
        WriteInteger(roomId);
        WriteInteger(playerId);
        WriteString(playerUsername);
    }
}