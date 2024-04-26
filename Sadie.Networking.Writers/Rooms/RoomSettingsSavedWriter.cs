using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomSettingsSavedWriter : AbstractPacketWriter
{
    public RoomSettingsSavedWriter(long roomId)
    {
        WriteShort(ServerPacketId.RoomSettingsSaved);
        WriteLong(roomId);
    }
}