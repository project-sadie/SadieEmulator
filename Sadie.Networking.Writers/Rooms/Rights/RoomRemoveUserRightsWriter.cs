using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Rights;

public class RoomRemoveUserRightsWriter : NetworkPacketWriter
{
    public RoomRemoveUserRightsWriter(long roomId, long playerId)
    {
        WriteShort(ServerPacketId.RoomRemoveUserRights);
        WriteLong(roomId);
        WriteLong(playerId);
    }
}