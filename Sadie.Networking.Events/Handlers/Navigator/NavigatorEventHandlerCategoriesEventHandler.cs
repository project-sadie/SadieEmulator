using Sadie.Game.Rooms.Categories;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

public class NavigatorEventHandlerCategoriesEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new NavigatorEventCategoriesWriter(new List<RoomCategory>()).GetAllBytes());
    }
}