using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Networking.Events.Handlers.Players.Inventory;

public class PlayerInventoryBadgesEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerInventoryBadges;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
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