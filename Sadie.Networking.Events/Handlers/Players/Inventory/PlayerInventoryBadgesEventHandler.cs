using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Networking.Events.Handlers.Players.Inventory;

[PacketId(EventHandlerId.PlayerInventoryBadges)]
public class PlayerInventoryBadgesEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var badges = client.Player.Badges.ToList();
        
        var equippedBadges = badges.
            Where(x => x.Slot is > 0 and <= 5).
            ToList();
        
        await client.WriteToStreamAsync(new PlayerInventoryBadgesWriter
        {
            Badges = badges,
            EquippedBadges = equippedBadges
        });
    }
}