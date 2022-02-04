using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Purse;

namespace Sadie.Networking.Packets.Client.Players;

public class PlayerBalanceEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter(1000).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(1000, 1000, 1000).GetAllBytes());
    }
}