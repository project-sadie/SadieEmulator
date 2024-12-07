using Sadie.API;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users;

[PacketId(ServerPacketId.RoomUserChat)]
public class RoomUserChatWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required string Message { get; init; }
    public required int EmotionId { get; init; }
    public required int ChatBubbleId { get; init; }
    public required int Unknown1 { get; init; }
    public required int MessageLength { get; init; }
}