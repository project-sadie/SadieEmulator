using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Club;

public class PlayerSubscriptionParser
{
    public string Name { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Name = reader.ReadString();
    }
}