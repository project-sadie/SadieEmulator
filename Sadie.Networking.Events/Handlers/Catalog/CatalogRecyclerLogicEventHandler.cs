using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogRecyclerLogicEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.CatalogRecyclerLogic;
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new CatalogRecyclerLogicWriter());
    }
}