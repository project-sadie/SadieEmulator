using Sadie.Networking.Serialization;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPurchaseFailedWriter : AbstractPacketWriter
{
    public required int Error { get; init; }
}