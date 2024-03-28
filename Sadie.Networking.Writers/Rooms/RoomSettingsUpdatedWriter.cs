using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomSettingsUpdatedWriter : NetworkPacketWriter
{
    public RoomSettingsUpdatedWriter(int roomId)
    {
        WriteShort(ServerPacketId.RoomSettingsUpdated);
        WriteInteger(roomId);
    }
}