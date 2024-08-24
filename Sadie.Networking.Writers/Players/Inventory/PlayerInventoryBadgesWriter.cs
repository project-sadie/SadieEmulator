using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryBadges)]
public class PlayerInventoryBadgesWriter : AbstractPacketWriter
{
    public required List<PlayerBadge> Badges { get; init; }
    public required List<PlayerBadge> EquippedBadges { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(Badges.Count);

        foreach (var playerBadge in Badges)
        {
            writer.WriteInteger(playerBadge.Id);
            writer.WriteString(playerBadge.Badge.Code);
        }
        
        writer.WriteInteger(EquippedBadges.Count);

        foreach (var playerBadge in EquippedBadges)
        {
            writer.WriteInteger(playerBadge.Slot);
            writer.WriteString(playerBadge.Badge.Code);
        }
    }
}