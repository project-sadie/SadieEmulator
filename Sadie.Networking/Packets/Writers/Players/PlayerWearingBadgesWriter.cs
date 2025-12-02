using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players;

[PacketId(ServerPacketId.PlayerBadges)]
public class PlayerWearingBadgesWriter : AbstractPacketWriter
{
    public required int PlayerId { get; init; }
    public required ICollection<PlayerBadgeDto> Badges { get; init; }

    public override async Task OnConfigureRulesAsync()
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