using Sadie.Database.Models.Rooms.Chat;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserShoutWriter : AbstractPacketWriter
{
    public RoomUserShoutWriter(RoomChatMessage message, int unknown1)
    {
        WriteShort(ServerPacketId.RoomUserShout);
        WriteInteger(message.PlayerId);
        WriteString(message.Message);
        WriteInteger(message.EmotionId);
        WriteInteger((int) message.ChatBubbleId);
        WriteInteger(unknown1);
        WriteInteger(message.Message.Length);
    }
}