using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerSanctionStatusEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerSanctionStatus;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerSanctionStatusWriter
        {
            HasPreviousSanction = false,
            OnProbation = false,
            LastSanctionType = "ALERT",
            SanctionTime = 0,
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