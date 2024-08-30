using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Rights;

[PacketId(ServerPacketId.RoomGiveUserRights)]
public class RoomGiveUserRightsWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
    public required int PlayerId { get; init; }
    public required string PlayerUsername { get; init; }
}