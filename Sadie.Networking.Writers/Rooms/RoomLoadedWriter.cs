namespace Sadie.Networking.Packets.Server.Rooms;

public class RoomLoadedWriter : NetworkPacketWriter
{
    public RoomLoadedWriter() : base(ServerPacketId.RoomLoaded)
    {
    }
}