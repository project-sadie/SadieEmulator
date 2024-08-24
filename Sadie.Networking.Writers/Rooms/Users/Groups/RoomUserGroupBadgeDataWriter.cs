using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Dtos;

namespace Sadie.Networking.Writers.Rooms.Users.Groups;

[PacketId(ServerPacketId.RoomUserGroupBadgeData)]
public class RoomUserGroupBadgeDataWriter : AbstractPacketWriter
{
    public required List<GroupBadgeData> BadgeData { get; set; }
}