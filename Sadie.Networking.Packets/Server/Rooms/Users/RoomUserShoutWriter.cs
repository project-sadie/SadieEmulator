using Sadie.Game.Rooms;

namespace Sadie.Networking.Packets.Server.Rooms.Users;

internal class RoomUserShoutWriter : NetworkPacketWriter
{
    internal RoomUserShoutWriter(RoomChatMessage message) : base(ServerPacketId.RoomUserShout)
    {
        WriteLong(message.Room.Id);
        WriteString(message.Message);
        WriteLong(message.EmotionId);
        WriteLong(message.BubbleId);
        WriteLong(0);
        WriteLong(message.Message.Length);
    }
}