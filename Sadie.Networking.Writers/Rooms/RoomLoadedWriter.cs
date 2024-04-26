using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomLoadedWriter : AbstractPacketWriter
{
    public RoomLoadedWriter()
    {
        WriteShort(ServerPacketId.RoomLoaded);
    }
}