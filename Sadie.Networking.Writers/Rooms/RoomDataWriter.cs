using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomDataWriter : NetworkPacketWriter
{
    public RoomDataWriter(int roomId, string modelName) : base(ServerPacketId.RoomData)
    {
        WriteString(modelName);
        WriteInt(roomId);
    }
}