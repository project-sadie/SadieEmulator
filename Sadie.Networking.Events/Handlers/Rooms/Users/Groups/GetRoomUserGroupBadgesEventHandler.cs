using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Groups;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Groups;

[PacketId(EventHandlerIds.PlayerGroupBadges)]
public class GetRoomUserGroupBadgesEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var badgeData = new List<GroupBadgeData>(); // TODO: Fetch
        
        await client.WriteToStreamAsync(new RoomUserGroupBadgeDataWriter
        {
            BadgeData = badgeData
        });
    }
}