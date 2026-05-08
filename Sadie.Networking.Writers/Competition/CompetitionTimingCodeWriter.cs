using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Competition;

[PacketId(ServerPacketId.CompetitionTimingCode)]
public class CompetitionTimingCodeWriter : AbstractPacketWriter
{
    public required string Schedule { get; init; }
    public required string Code { get; init; }
}