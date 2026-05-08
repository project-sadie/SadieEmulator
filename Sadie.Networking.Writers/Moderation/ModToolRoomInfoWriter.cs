using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Moderation;

[PacketId(ServerPacketId.ModToolsRoomInfo)]
public class ModToolRoomInfoWriter : AbstractPacketWriter
{
    public required int Id { get; set; }
    public required int UserCount { get; set; }
    public required bool OwnerInRoom { get; set; }
    public required long OwnerId { get; set; }
    public required string OwnerName { get; set; }
    public required bool Unknown1 { get; set; }
    public required string Name{ get; set; }
    public required string Description { get; set; }
    public required List<string> Tags { get; set; }
}