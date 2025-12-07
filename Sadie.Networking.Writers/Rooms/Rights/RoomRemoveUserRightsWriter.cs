using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Rights;

[PacketId(ServerPacketId.RoomRemoveUserRights)]
public class RoomRemoveUserRightsWriter : AbstractPacketWriter
{
    public required long RoomId { get; init; }
    public required long PlayerId { get; init; }
}