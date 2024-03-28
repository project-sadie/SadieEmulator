using Sadie.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomSettingsErrorWriter : NetworkPacketWriter
{
    public RoomSettingsErrorWriter(int roomId, RoomSettingsError error, string unknown = "")
    {
        WriteShort(ServerPacketId.RoomSettingsError);
        WriteInteger(roomId);
        WriteInteger((int) error);
        WriteString(unknown);
    }
}