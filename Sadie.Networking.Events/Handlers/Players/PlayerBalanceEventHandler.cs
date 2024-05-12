using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Purse;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerBalanceEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerBalance;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerData = client.Player.Data;
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
        {
            Credits = playerData.CreditBalance
        });
        
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
        {
            PixelBalance = playerData.PixelBalance,
            SeasonalBalance = playerData.SeasonalBalance,
            GotwPoints = playerData.GotwPoints
        });
    }
}