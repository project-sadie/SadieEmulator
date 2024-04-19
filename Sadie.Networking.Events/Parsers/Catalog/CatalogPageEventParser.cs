using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Catalog;

public class CatalogPageEventParser : INetworkPacketEventParser
{
    public int PageId { get; private set; }
    public int OfferId { get; private set; }
    public string CatalogMode { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        PageId = reader.ReadInt();
        OfferId = reader.ReadInt();
        CatalogMode = reader.ReadString();
    }
}