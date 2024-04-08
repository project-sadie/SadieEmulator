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
        
        var currencies = new Dictionary<int, long>
        {
            {0, playerData.PixelBalance},
            {1, 0}, // snowflakes
            {2, 0}, // hearts
            {3, 0}, // gift points
            {4, 0}, // shells
            {5, playerData.SeasonalBalance},
            {101, 0}, // snowflakes
            {102, 0}, // unknown
            {103, playerData.GotwPoints},
            {104, 0}, // unknown
            {105, 0} // unknown
        };
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter(playerData.CreditBalance).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(currencies).GetAllBytes());
    }
}