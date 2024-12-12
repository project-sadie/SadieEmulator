using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users;

[PacketId(ServerPacketId.RoomUserWhisper)]
public class RoomUserWhisperWriter : AbstractPacketWriter
{
    public required int SenderId { get; set; }
    public required string Message { get; init; }
    public required int EmotionId { get; set; }
    public required int ChatBubbleId { get; set; }
    public required List<string> Urls { get; init; }
    public required int MessageLength { get; init; }
}