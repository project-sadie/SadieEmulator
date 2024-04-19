using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogIndexEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.CatalogIndex;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        
    }
}