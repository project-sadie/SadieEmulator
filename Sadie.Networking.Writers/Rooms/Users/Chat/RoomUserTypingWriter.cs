using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users.Chat;

public class RoomUserTypingWriter : NetworkPacketWriter
{
    public RoomUserTypingWriter(long userId, bool isTyping)
    {
        WriteShort(ServerPacketId.RoomUserTyping);
        WriteLong(userId);
        WriteInteger(isTyping ? 1 : 0);
    }
}