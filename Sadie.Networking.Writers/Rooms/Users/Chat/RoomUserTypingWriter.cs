using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users.Chat;

public class RoomUserTypingWriter : AbstractPacketWriter
{
    public RoomUserTypingWriter(long userId, bool isTyping)
    {
        WriteShort(ServerPacketId.RoomUserTyping);
        WriteLong(userId);
        WriteInteger(isTyping ? 1 : 0);
    }
}