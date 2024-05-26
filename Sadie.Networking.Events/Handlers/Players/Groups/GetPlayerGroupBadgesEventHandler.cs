using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Groups;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Groups;

[PacketId(EventHandlerIds.PlayerGroupBadges)]
public class GetPlayerGroupBadgesEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var badgeData = new List<GroupBadgeData>(); // TODO: Fetch
        
        await client.WriteToStreamAsync(new PlayerGroupBadgesWriter
        {
            BadgeData = badgeData
        });
    }
}