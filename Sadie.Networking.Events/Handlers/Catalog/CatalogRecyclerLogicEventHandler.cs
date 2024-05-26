using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerIds.CatalogRecyclerLogic)]
public class CatalogRecyclerLogicEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new CatalogRecyclerLogicWriter
        {
            PrizeSize = 0
        });
    }
}