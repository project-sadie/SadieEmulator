using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Catalog;

public class CatalogIndexEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var isBuildersClub = reader.ReadString() == "BUILDERS_CLUB";
        var type = isBuildersClub ? 1 : 0;
        
        await client.WriteToStreamAsync(new CatalogIndexWriter(type).GetAllBytes());
    }
}