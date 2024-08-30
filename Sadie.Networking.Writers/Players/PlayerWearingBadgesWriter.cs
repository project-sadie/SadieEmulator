using Sadie.API.Networking;
using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerBadges)]
public class PlayerWearingBadgesWriter : AbstractPacketWriter
{
    public required int PlayerId { get; init; }
    public required ICollection<PlayerBadge> Badges { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Badges))!, writer =>
        {
            writer.WriteInteger(Badges.Count);

            foreach (var item in Badges)
            {
                writer.WriteInteger(item.Slot);
                writer.WriteString(item.Badge.Code);
            }
        });
    }
}