using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Purse;

[PacketId(ServerPacketId.PlayerCreditsBalance)]
public class PlayerCreditsBalanceWriter : AbstractPacketWriter
{
    public required long Credits { get; init; }

    public override void OnConfigureRules()
    {
        Convert<string>(
            GetType().GetProperty(nameof(Credits))!,
            i => (long) i + ".0");
    }
}