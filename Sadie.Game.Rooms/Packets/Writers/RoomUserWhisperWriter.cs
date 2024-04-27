using Sadie.Networking.Serialization;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserWhisperWriter : AbstractPacketWriter
{
    public required int SenderId { get; set; }
    public required string Message { get; set; }
    public required int EmotionId { get; set; }
    public required int Bubble { get; set; }
}