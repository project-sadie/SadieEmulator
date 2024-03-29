using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Navigator;

public class NavigatorSearchParser
{
    public string TabName { get; private set; }
    public string SearchQuery { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        TabName = reader.ReadString();
        SearchQuery = reader.ReadString();
    }
}