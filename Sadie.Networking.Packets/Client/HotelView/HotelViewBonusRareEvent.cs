using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.HotelView;

namespace Sadie.Networking.Packets.Client.HotelView;

public class HotelViewBonusRareEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new HotelViewBonusRare().GetAllBytes());
    }
}