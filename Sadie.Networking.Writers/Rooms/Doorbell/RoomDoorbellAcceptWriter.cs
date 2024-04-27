using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

public class RoomDoorbellAcceptWriter : AbstractPacketWriter
{
    public RoomDoorbellAcceptWriter(string username)
    {
        WriteShort(ServerPacketId.RoomDoorbellAccept);
        WriteString(username);
    }
}