using Sadie.Game.Rooms.Chat;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserChatWriter : NetworkPacketWriter
{
    public RoomUserChatWriter(RoomChatMessage message)
    {
        WriteShort(ServerPacketId.RoomUserChat);
        WriteLong(message.Sender.Id);
        WriteString(message.Message);
        WriteLong(message.EmotionId);
        WriteLong((int) message.Bubble);
        WriteLong(0);
        WriteLong(message.Message.Length);
    }
}