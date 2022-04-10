using Sadie.Game.Rooms.Chat;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserShoutWriter : NetworkPacketWriter
{
    public RoomUserShoutWriter(RoomChatMessage message, int unknown1)
    {
        WriteShort(ServerPacketId.RoomUserShout);
        WriteInt(message.Sender.Id);
        WriteString(message.Message);
        WriteInt(message.EmotionId);
        WriteInt((int) message.Bubble);
        WriteInt(unknown1);
        WriteInt(message.Message.Length);
    }
}