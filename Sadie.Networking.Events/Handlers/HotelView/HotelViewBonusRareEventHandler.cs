using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
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