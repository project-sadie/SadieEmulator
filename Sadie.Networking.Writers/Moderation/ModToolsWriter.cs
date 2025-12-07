using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Moderation;

[PacketId(ServerPacketId.ModTools)]
public class ModToolsWriter : AbstractPacketWriter
{
    public required List<IssueData> Issues { get; set; }
    public required List<string> MessageTemplates { get; set; }
    public required int Unknown3 { get; set; }
    public required bool CallForHelpPermission { get; set; }
    public required bool ChatLogsPermission { get; set; }
    public required bool AlertPermission { get; set; }
    public required bool KickPermission { get; set; }
    public required bool BanPermission { get; set; }
    public required bool RoomAlertPermission { get; set; }
    public required bool RoomKickPermission { get; set; }
    public required List<string> RoomMessageTemplates { get; set; }
}