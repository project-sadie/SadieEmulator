using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserWhisper)]
public class RoomUserWhisperWriter : AbstractPacketWriter
{
    public required long SenderId { get; set; }
    public required string Message { get; init; }
    public required int EmotionId { get; set; }
    public required int ChatBubbleId { get; set; }
    public required List<string> Urls { get; init; }
    public required int MessageLength { get; init; }
}