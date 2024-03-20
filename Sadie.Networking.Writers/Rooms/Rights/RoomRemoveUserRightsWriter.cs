using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Rights;

public class RoomRemoveUserRightsWriter : NetworkPacketWriter
{
    public RoomRemoveUserRightsWriter(int roomId, int playerId)
    {
        WriteShort(ServerPacketId.RoomRemoveUserRights);
        WriteInteger(roomId);
        WriteInteger(playerId);
    }
}