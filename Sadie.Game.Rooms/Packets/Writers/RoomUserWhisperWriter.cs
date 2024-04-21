using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

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