using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomDataWriter : NetworkPacketWriter
{
    public RoomDataWriter(int roomId, string layoutName)
    {
        WriteShort(ServerPacketId.RoomData);
        WriteString(layoutName);
        WriteInteger(roomId);
    }
}