using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Rights;

[PacketId(ServerPacketId.RoomRemoveUserRights)]
public class RoomRemoveUserRightsWriter : AbstractPacketWriter
{
    public required long RoomId { get; init; }
    public required long PlayerId { get; init; }
}