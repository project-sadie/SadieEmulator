using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Catalog;

public class CatalogPurchaseEventParser : INetworkPacketEventParser
{
    public int PageId { get; private set; }
    public int ItemId { get; private set; }
    public string ExtraData { get; private set; }
    public int Amount { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        PageId = reader.ReadInt();
        ItemId = reader.ReadInt();
        ExtraData = reader.ReadString();
        Amount = reader.ReadInt();
    }
}