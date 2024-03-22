using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

public class RoomDoorbellWriter : NetworkPacketWriter
{
    public RoomDoorbellWriter()
    {
        WriteShort(ServerPacketId.RoomDoorbell);
        WriteString("");
    }
    
    public RoomDoorbellWriter(string username)
    {
        WriteShort(ServerPacketId.RoomDoorbell);
        WriteString(username);
    }
}