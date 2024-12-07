using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.NavigatorPromotedRooms)]
public class NavigatorPromotedRoomsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new NavigatorPromotedRoomsWriter
        {
            Unknown1 = 2,
            Unknown2 = "",
            Unknown3 = 0,
            Unknown4 = true,
            Unknown5 = 0,
            Unknown6 = "A",
            Unknown7 = "B",
            Unknown8 = 1,
            Unknown9 = "C",
            Unknown10 = "D",
            Unknown11 = 1,
            Unknown12 = 1,
            Unknown13 = 1,
            Unknown14 = "E"
        });
    }
}