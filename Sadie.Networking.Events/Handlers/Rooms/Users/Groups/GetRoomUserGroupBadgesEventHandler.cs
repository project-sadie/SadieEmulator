using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Dtos;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Networking.Writers.Rooms.Users.Groups;
using Sadie.Shared.Attributes;

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