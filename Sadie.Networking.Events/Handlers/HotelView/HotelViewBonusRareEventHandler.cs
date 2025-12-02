using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.HotelView;

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