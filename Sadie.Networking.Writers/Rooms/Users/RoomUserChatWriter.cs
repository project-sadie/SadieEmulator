using Sadie.Database.Models.Rooms.Chat;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserChat)]
public class RoomUserChatWriter : AbstractPacketWriter
{
    public required RoomChatMessage Message { get; init; }
    public required int Unknown1 { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(Message.PlayerId);
        writer.WriteString(Message.Message);
        writer.WriteInteger(Message.EmotionId);
        writer.WriteInteger((int) Message.ChatBubbleId);
        writer.WriteInteger(Unknown1);
        writer.WriteInteger(Message.Message.Length);
    }
}