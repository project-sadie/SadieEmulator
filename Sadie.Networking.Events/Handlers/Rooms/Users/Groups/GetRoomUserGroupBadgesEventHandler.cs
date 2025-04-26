using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Dtos;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Groups;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Groups;

[PacketId(EventHandlerId.PlayerGroupBadges)]
public class GetRoomUserGroupBadgesEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var badgeData = new List<IGroupBadgeData>(); // TODO: Fetch
        
        await client.WriteToStreamAsync(new RoomUserGroupBadgeDataWriter
        {
            BadgeData = badgeData
        });
    }
}