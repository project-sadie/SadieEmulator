using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Purse;

namespace Sadie.Networking.Packets.Client.Players;

public class RequestPlayerBalancePacket : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerCreditsBalance(1000000).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerActivityPointsBalance(1000000, 1000000, 1000000).GetAllBytes());
    }
}