using Sadie.Game.Rooms;

namespace Sadie.Networking.Packets.Server.Rooms.Users;

public class RoomUserShoutWriter : NetworkPacketWriter
{
    public RoomUserShoutWriter(RoomChatMessage message) : base(ServerPacketId.RoomUserShout)
    {
        WriteLong(message.Sender.Id);
        WriteString(message.Message);
        WriteLong(message.EmotionId);
        WriteLong(message.BubbleId);
        WriteLong(0);
        WriteLong(message.Message.Length);
    }
}