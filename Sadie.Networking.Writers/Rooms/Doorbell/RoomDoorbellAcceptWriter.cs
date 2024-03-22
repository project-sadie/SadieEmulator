using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

public class RoomDoorbellAcceptWriter : NetworkPacketWriter
{
    public RoomDoorbellAcceptWriter(string username)
    {
        WriteShort(ServerPacketId.RoomDoorbellAccept);
        WriteString(username);
    }
}