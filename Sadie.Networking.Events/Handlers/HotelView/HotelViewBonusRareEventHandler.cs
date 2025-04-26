using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.HotelView;

namespace Sadie.Networking.Events.Handlers.HotelView;

[PacketId(EventHandlerId.HotelViewBonusRare)]
public class HotelViewBonusRareEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new HotelViewBonusRareWriter
        {
            Name = "",
            Id = -1,
            Coins = -1,
            CoinsRequiredToBuy = -1
        });
    }
}