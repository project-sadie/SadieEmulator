using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users.Chat;

public class RoomUserWhisperWriter : NetworkPacketWriter
{
    public RoomUserWhisperWriter(int senderId, string message, int emotionId, int bubble)
    {
        WriteShort(ServerPacketId.RoomUserWhisper);
        WriteLong(senderId);
        WriteString(message);
        WriteLong(emotionId);
        WriteLong(bubble);
        WriteLong(0);
        WriteLong(message.Length);
    }
}