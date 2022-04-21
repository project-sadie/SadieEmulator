using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomSettingsSavedWriter : NetworkPacketWriter
{
    public RoomSettingsSavedWriter(int roomId)
    {
        WriteShort(ServerPacketId.RoomSettingsSaved);
        WriteInteger(roomId);
    }
}