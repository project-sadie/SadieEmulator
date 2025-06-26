using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerSanctionStatus)]
public class PlayerSanctionStatusEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new PlayerSanctionStatusWriter
        {
            HasPreviousSanction = false,
            OnProbation = false,
            SanctionName = "ALERT",
            SanctionLengthHours = 0,
            Unknown1 = 30,
            Reason = "cfh.reason.EMPTY",
            ProbationStart = DateTime.Now,
            Unknown2 = 0,
            NextSanctionType = "ALERT",
            HoursForNextSanction = 0,
            Unknown3 = 30,
            Muted = false,
            TradeLockedUntil = DateTime.MinValue
        });
    }
}