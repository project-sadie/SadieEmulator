using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomOwnerWriter : NetworkPacketWriter
{
    public RoomOwnerWriter()
    {
        WriteShort(ServerPacketId.RoomOwner);
    }
}