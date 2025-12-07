using Sadie.API.Interfaces.Networking;
using Sadie.API.Interfaces.Networking.Events.Dtos;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users.Groups;

[PacketId(ServerPacketId.RoomUserGroupBadgeData)]
public class RoomUserGroupBadgeDataWriter : AbstractPacketWriter
{
    public required List<IGroupBadgeData> BadgeData { get; set; }
}