using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Messenger;

public class PlayerSearchParser
{
    public string SearchQuery { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        SearchQuery = reader.ReadString();
    }
}