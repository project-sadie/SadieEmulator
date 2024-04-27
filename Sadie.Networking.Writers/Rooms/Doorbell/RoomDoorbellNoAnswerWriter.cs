using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

public class RoomDoorbellNoAnswerWriter : AbstractPacketWriter
{
    public RoomDoorbellNoAnswerWriter(string username)
    {
        WriteShort(ServerPacketId.RoomDoorbellNoAnswer);
        WriteString(username);
    }
}