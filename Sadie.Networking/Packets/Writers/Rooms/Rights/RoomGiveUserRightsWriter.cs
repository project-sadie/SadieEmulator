using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Rights;

[PacketId(ServerPacketId.RoomGiveUserRights)]
public class RoomGiveUserRightsWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
    public required int PlayerId { get; init; }
    public required string PlayerUsername { get; init; }
}