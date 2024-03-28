using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomLoadedWriter : NetworkPacketWriter
{
    public RoomLoadedWriter()
    {
        WriteShort(ServerPacketId.RoomLoaded);
    }
}