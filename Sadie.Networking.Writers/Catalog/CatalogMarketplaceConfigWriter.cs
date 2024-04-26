using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogMarketplaceConfigWriter : AbstractPacketWriter
{
    public CatalogMarketplaceConfigWriter(
        bool unknown, 
        int commissionPercent,
        int credits, 
        int advertisements,
        int minPrice, 
        int maxPrice, 
        int hoursInMarketplace, 
        int daysToDisplay)
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