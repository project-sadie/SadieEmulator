using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerPingParser
{
    public int Id { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Id = reader.ReadInteger();
    }
}