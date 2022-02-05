using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Purse;

namespace Sadie.Networking.Packets.Client.Players;

public class PlayerBalanceEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var balance = client.Player.Balance;
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter(balance.Credits).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(balance.Pixels, balance.Seasonal, balance.Gotw).GetAllBytes());
    }
}