using Sadie.Database.Models.Rooms.Chat;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserChatWriter : NetworkPacketWriter
{
    public RoomUserChatWriter(RoomChatMessage message, int unknown1)
    {
        WriteShort(ServerPacketId.RoomUserChat);
        WriteInteger(message.PlayerId);
        WriteString(message.Message);
        WriteInteger(message.EmotionId);
        WriteInteger((int) message.ChatBubbleId);
        WriteInteger(unknown1);
        WriteInteger(message.Message.Length);
    }
}