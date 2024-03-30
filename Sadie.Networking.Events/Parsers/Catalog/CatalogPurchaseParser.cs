using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Catalog;

public class CatalogPurchaseParser
{
    public int PageId { get; private set; }
    public int ItemId { get; private set; }
    public string ExtraData { get; private set; }
    public int Amount { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        PageId = reader.ReadInteger();
        ItemId = reader.ReadInteger();
        ExtraData = reader.ReadString();
        Amount = reader.ReadInteger();
    }
}