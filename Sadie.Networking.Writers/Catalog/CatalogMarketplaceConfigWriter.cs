using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogMarketplaceConfigWriter : NetworkPacketWriter
{
    public CatalogMarketplaceConfigWriter(bool unknown, int commissionPercent, int credits, int advertisements, int minPrice, int maxPrice, int hoursInMarketplace, int daysToDisplay)
    {
        WriteShort(ServerPacketId.CatalogMarketplaceConfig);
        WriteBool(unknown);
        WriteInteger(commissionPercent);
        WriteInteger(credits);
        WriteInteger(advertisements);
        WriteInteger(minPrice);
        WriteInteger(maxPrice);
        WriteInteger(hoursInMarketplace);
        WriteInteger(daysToDisplay);
    }
}