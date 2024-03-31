using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Catalog;

public class CatalogModeEventParser : INetworkPacketEventParser
{
    public string Mode { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        Mode = reader.ReadString();
    }
}