using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryBadgesWriter : NetworkPacketWriter
{
    public PlayerInventoryBadgesWriter(List<PlayerBadge> badges, List<PlayerBadge> equippedBadges)
    {
        WriteShort(ServerPacketId.PlayerInventoryBadges);
        
        WriteInteger(badges.Count);

        foreach (var playerBadge in badges)
        {
            WriteInteger(playerBadge.Id);
            WriteString(playerBadge.Badge.Code);
        }
        
        WriteInteger(equippedBadges.Count);

        foreach (var playerBadge in equippedBadges)
        {
            WriteInteger(playerBadge.Slot);
            WriteString(playerBadge.Badge.Code);
        }
    }
}