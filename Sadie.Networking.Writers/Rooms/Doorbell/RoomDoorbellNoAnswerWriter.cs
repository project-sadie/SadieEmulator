using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

public class RoomDoorbellNoAnswerWriter : NetworkPacketWriter
{
    public RoomDoorbellNoAnswerWriter(string username)
    {
        WriteShort(ServerPacketId.RoomDoorbellNoAnswer);
        WriteString(username);
    }
}