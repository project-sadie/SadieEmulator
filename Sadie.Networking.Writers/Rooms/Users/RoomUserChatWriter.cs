using Sadie.Game.Rooms.Chat;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserChatWriter : NetworkPacketWriter
{
    public RoomUserChatWriter(RoomChatMessage message, int unknown1)
    {
        WriteShort(ServerPacketId.RoomUserChat);
        WriteInteger(message.Sender.Id);
        WriteString(message.Message);
        WriteInteger(message.EmotionId);
        WriteInteger((int) message.Bubble);
        WriteInteger(unknown1);
        WriteInteger(message.Message.Length);
    }
}