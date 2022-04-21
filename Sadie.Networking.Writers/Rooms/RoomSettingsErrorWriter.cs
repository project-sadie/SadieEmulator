using Sadie.Game.Rooms;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

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