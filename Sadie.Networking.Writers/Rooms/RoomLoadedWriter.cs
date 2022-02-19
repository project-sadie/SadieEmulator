using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomLoadedWriter : NetworkPacketWriter
{
    public RoomLoadedWriter() : base(ServerPacketId.RoomLoaded)
    {
    }
}