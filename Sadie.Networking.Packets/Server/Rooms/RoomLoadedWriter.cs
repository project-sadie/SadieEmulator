namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomLoadedWriter : NetworkPacketWriter
{
    internal RoomLoadedWriter() : base(ServerPacketId.LoadedRoom)
    {
    }
}