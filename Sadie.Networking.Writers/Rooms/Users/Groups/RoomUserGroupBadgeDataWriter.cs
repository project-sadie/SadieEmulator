using Sadie.API.Networking;
using Sadie.API.Networking.Events.Dtos;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users.Groups;

[PacketId(ServerPacketId.RoomUserGroupBadgeData)]
public class RoomUserGroupBadgeDataWriter : AbstractPacketWriter
{
    public required List<IGroupBadgeData> BadgeData { get; set; }
}