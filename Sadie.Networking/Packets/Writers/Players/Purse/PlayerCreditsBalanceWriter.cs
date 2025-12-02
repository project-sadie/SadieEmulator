using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players.Purse;

[PacketId(ServerPacketId.PlayerCreditsBalance)]
public class PlayerCreditsBalanceWriter : AbstractPacketWriter
{
    public required long Credits { get; init; }

    public override async Task OnConfigureRulesAsync()
    {
        Convert<string>(
            GetType().GetProperty(nameof(Credits))!,
            i => (long) i + ".0");
    }
}