using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

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