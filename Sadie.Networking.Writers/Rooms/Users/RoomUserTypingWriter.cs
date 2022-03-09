using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserTypingWriter : NetworkPacketWriter
{
    public RoomUserTypingWriter(long userId, bool isTyping)
    {
        WriteShort(ServerPacketId.RoomUserTyping);
        WriteLong(userId);
        WriteInt(isTyping ? 1 : 0);
    }
}