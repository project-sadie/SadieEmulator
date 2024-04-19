using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

public class PromotedRoomsEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PromotedRooms;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new NavigatorPromotedRoomsWriter());
    }
}