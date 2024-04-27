using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms.Rights;

[PacketId(ServerPacketId.RoomRemoveUserRights)]
public class RoomRemoveUserRightsWriter : AbstractPacketWriter
{
    public required long RoomId { get; init; }
    public required long PlayerId { get; init; }
}