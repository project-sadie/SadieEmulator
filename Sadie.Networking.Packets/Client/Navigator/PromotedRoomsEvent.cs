using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Navigator;

namespace Sadie.Networking.Packets.Client.Navigator;

public class PromotedRoomsEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PromotedRooms().GetAllBytes());
    }
}