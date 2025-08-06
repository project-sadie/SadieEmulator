using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Networking.Events.Handlers.Players.Inventory;

[PacketId(EventHandlerId.PlayerInventoryBadges)]
public class PlayerInventoryBadgesEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var badges = client.Player.Badges
            .ToDictionary(x => x.Id, x => x.Badge?.Code ?? "");
        
        var equippedBadges = client.Player.Badges
            .Where(x => x.Slot is > 0 and <= 5)
            .ToDictionary(x => x.Id, x => x.Badge?.Code ?? "");
        
        await client.WriteToStreamAsync(new PlayerInventoryBadgesWriter
        {
            Badges = badges,
            EquippedBadges = equippedBadges
        });
    }
}