using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomOwnerWriter : AbstractPacketWriter
{
    public RoomOwnerWriter()
    {
        WriteShort(ServerPacketId.RoomOwner);
    }
}