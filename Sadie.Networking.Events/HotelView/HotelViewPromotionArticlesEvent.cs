using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.HotelView;

namespace Sadie.Networking.Events.HotelView;

public class HotelViewPromotionsEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new HotelViewPromotionsWriter().GetAllBytes());
    }
}