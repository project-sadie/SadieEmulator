using Sadie.Game.Rooms.Categories;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Navigator;

namespace Sadie.Networking.Packets.Client.Navigator;

public class NavigatorEventCategoriesEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new NavigatorEventCategoriesWriter(new List<RoomCategory>()).GetAllBytes());
    }
}