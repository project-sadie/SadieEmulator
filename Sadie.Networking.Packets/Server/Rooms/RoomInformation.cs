namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomInformation : NetworkPacketWriter
{
    internal RoomInformation(int roomId) : base(ServerPacketIds.RoomInformation)
    {
        WriteString("model");
        WriteInt(roomId);
    }
}