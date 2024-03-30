using Sadie.Game.Rooms.Chat;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserShoutWriter : NetworkPacketWriter
{
    public RoomUserShoutWriter(RoomChatMessage message, int unknown1)
    {
        WriteShort(ServerPacketId.RoomUserShout);
        WriteInteger(message.Sender.Id);
        WriteString(message.Message);
        WriteInteger(message.EmotionId);
        WriteInteger((int) message.Bubble);
        WriteInteger(unknown1);
        WriteInteger(message.Message.Length);
    }
}