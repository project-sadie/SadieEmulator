using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerPingEventParser : INetworkPacketEventParser
{
    public int Id { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Id = reader.ReadInt();
    }
}