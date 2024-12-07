using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerSanctionStatus)]
public class PlayerSanctionStatusWriter : AbstractPacketWriter
{
    public required bool HasPreviousSanction { get; init; }
    public required bool OnProbation { get; init; }
    public required string SanctionName { get; init; }
    public required int SanctionLengthHours { get; init; }
    public required int Unknown1 { get; init; }
    public required string Reason { get; init; }
    public required DateTime ProbationStart { get; init; }
    public required int Unknown2 { get; init; }
    public required string NextSanctionType { get; init; }
    public required int HoursForNextSanction { get; init; }
    public required int Unknown3 { get; init; }
    public required bool Muted { get; init; }
    public required DateTime TradeLockedUntil { get; init; }

    public override void OnConfigureRules()
    {
        Convert<string>(GetType().GetProperty(nameof(ProbationStart))!, o => ((DateTime)o).ToString());
        
        Override(GetType().GetProperty(nameof(TradeLockedUntil))!, writer =>
        {
            writer.WriteString(TradeLockedUntil == DateTime.MinValue ? "" : TradeLockedUntil.ToString());
        });
    }
}