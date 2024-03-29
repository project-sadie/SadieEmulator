using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomSettingsUpdatedWriter : NetworkPacketWriter
{
    public RoomSettingsUpdatedWriter(long roomId)
    {
        WriteShort(ServerPacketId.RoomSettingsUpdated);
        WriteLong(roomId);
    }
}