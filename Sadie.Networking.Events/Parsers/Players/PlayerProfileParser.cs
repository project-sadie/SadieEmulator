using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerProfileParser
{
    public int ProfileId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        ProfileId = reader.ReadInteger();
    }
}