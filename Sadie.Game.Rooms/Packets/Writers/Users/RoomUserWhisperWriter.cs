using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users;

[PacketId(ServerPacketId.RoomUserWhisper)]
public class RoomUserWhisperWriter : AbstractPacketWriter
{
    public required int SenderId { get; set; }
    public required string Message { get; init; }
    public required int EmotionId { get; set; }
    public required int Bubble { get; set; }
    public required int Unknown { get; init; }

    public override void OnConfigureRules()
    {
        After(GetType().GetProperty(nameof(Bubble))!, writer =>
        {
            writer.WriteInteger(Unknown);
            writer.WriteInteger(Message.Length);
        });
    }
}