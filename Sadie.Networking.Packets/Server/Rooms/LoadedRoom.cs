namespace Sadie.Networking.Packets.Server.Rooms;

internal class LoadedRoom : NetworkPacketWriter
{
    internal LoadedRoom() : base(ServerPacketId.LoadedRoom)
    {
    }
}