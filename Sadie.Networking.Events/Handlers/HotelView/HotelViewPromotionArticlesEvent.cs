using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.HotelView;

public class HotelViewPromotionsEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.HotelViewPromotions;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
    }
}