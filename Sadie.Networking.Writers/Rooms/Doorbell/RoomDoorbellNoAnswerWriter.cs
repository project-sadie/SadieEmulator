using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

public class RoomDoorbellNoAnswerWriter : AbstractPacketWriter
{
    public RoomDoorbellNoAnswerWriter(string username)
    {
        WriteShort(ServerPacketId.RoomDoorbellNoAnswer);
        WriteString(username);
    }
}