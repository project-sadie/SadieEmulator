using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms.Rights;

[PacketId(ServerPacketId.RoomGiveUserRights)]
public class RoomGiveUserRightsWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
    public required int PlayerId { get; init; }
    public required string PlayerUsername { get; init; }
}