using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.HotelView;

namespace Sadie.Networking.Events.Handlers.HotelView;

[PacketId(EventHandlerIds.HotelViewBonusRare)]
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