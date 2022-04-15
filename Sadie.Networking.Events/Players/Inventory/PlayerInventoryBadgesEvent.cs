using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Networking.Events.Players.Inventory;

public class PlayerInventoryBadgesEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerData = client.Player!.Data;
        
        var badges = playerData.Badges;
        
        var equippedBadges = badges.
            Where(x => x.Slot is > 0 and <= 5).
            ToList();
        
        await client.WriteToStreamAsync(new PlayerInventoryBadgesWriter(badges, equippedBadges).GetAllBytes());
    }
}