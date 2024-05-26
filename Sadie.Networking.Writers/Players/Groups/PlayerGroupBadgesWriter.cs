using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Groups;

[PacketId(ServerPacketId.PlayerGroupBadges)]
public class PlayerGroupBadgesWriter : AbstractPacketWriter
{
    public required List<GroupBadgeData> BadgeData { get; set; }
}