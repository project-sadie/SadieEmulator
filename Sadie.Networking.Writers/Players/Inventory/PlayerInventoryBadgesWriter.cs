using Sadie.Game.Players.Badges;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryBadgesWriter : NetworkPacketWriter
{
    public PlayerInventoryBadgesWriter(List<PlayerBadge> badges, List<PlayerBadge> equippedBadges)
    {
        WriteShort(ServerPacketId.PlayerInventoryBadges);
        
        WriteInteger(badges.Count);

        foreach (var item in badges)
        {
            WriteInteger(item.Id);
            WriteString(item.Code);
        }
        
        WriteInteger(equippedBadges.Count);

        foreach (var item in equippedBadges)
        {
            WriteInteger(item.Slot);
            WriteString(item.Code);
        }
    }
}