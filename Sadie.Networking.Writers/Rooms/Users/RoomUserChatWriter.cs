using Sadie.Game.Rooms.Chat;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserChatWriter : NetworkPacketWriter
{
    public RoomUserChatWriter(RoomChatMessage message, int unknown1)
    {
        WriteShort(ServerPacketId.RoomUserChat);
        WriteInt(message.Sender.Id);
        WriteString(message.Message);
        WriteInt(message.EmotionId);
        WriteInt((int) message.Bubble);
        WriteInt(unknown1);
        WriteInt(message.Message.Length);
    }
}