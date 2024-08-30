using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

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