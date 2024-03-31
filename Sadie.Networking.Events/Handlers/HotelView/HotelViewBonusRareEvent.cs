using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.HotelView;

namespace Sadie.Networking.Events.Handlers.HotelView;

public class HotelViewBonusRareEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new HotelViewBonusRareWriter(
            "", 
            -1, 
            -1, 
            -1).GetAllBytes());
    }
}