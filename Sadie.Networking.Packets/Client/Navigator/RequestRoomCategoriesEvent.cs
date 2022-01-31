using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Navigator;

namespace Sadie.Networking.Packets.Client.Navigator;

public class RequestRoomCategoriesEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new RoomCategories().GetAllBytes());
    }
}