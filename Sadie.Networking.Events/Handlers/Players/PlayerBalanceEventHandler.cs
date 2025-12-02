using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Players.Purse;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerBalance)]
public class PlayerBalanceEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var playerData = client.Player.Player.Data;
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
        {
            Credits = playerData.CreditBalance
        });
        
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
        {
            Currencies = NetworkPacketEventHelpers.GetPlayerCurrencyMapFromData(playerData)
        });
    }
}