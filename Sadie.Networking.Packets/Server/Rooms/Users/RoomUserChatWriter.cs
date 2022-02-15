using Sadie.Game.Rooms;

namespace Sadie.Networking.Packets.Server.Rooms.Users;

internal class RoomUserChatWriter : NetworkPacketWriter
{
    internal RoomUserChatWriter(RoomChatMessage message) : base(ServerPacketId.RoomUserChat)
    {
        WriteLong(message.Sender.Id);
        WriteString(message.Message);
        WriteLong(message.EmotionId);
        WriteLong(message.BubbleId);
        WriteLong(0);
        WriteLong(message.Message.Length);
    }
}