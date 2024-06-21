using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.PromotedRooms)]
public class PromotedRoomsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new NavigatorPromotedRoomsWriter());
    }
}