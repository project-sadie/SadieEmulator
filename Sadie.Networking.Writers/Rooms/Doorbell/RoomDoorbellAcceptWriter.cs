using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

public class RoomDoorbellAcceptWriter : AbstractPacketWriter
{
    public RoomDoorbellAcceptWriter(string username)
    {
        WriteShort(ServerPacketId.RoomDoorbellAccept);
        WriteString(username);
    }
}