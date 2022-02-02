namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomDataWriter : NetworkPacketWriter
{
    internal RoomDataWriter(int roomId, string modelName) : base(ServerPacketId.RoomData)
    {
        WriteString(modelName);
        WriteInt(roomId);
    }
}