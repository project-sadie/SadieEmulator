using Sadie.API.DTOs.Rooms.Chat;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Moderation;

[PacketId(ServerPacketId.ModToolsRoomChatLog)]
public class ModToolRoomChatLogWriter : AbstractPacketWriter
{
    public required byte Unknown1 { get; init; }
    public required short Unknown2 { get; set; }
    public required string Unknown3 { get; set; }
    public required byte Unknown4 { get; init; }
    public required string Unknown5 { get; set; }
    public required string Unknown6 { get; set; }
    public required byte Unknown7 { get; init; }
    public required int Unknown8 { get; set; }
    public required List<RoomChatMessageDto> Messages { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Unknown1))!, writer => { writer.WriteByte(Unknown1); });
        Override(GetType().GetProperty(nameof(Unknown4))!, writer => { writer.WriteByte(Unknown4); });
        Override(GetType().GetProperty(nameof(Unknown7))!, writer => { writer.WriteByte(Unknown7); });
        
        Override(GetType().GetProperty(nameof(Messages))!, writer =>
        {
            writer.WriteInteger(Messages.Count);

            foreach (var message in Messages)
            {
                writer.WriteString(message.CreatedAt.ToString("HH:mm"));
                writer.WriteLong(message.PlayerId);
                writer.WriteString(message.Player.Username);
                writer.WriteString(message.Message ?? "Unable to display message");
                writer.WriteBool(false);
            }
        });
    }
}