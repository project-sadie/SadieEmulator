using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Purse;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerBalance)]
public class PlayerBalanceEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
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