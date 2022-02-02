namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomDataWriter : NetworkPacketWriter
{
    internal RoomDataWriter(int roomId) : base(ServerPacketId.RoomInformation)
    {
        WriteString("model");
        WriteInt(roomId);
    }
}