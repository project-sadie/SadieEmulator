using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerWearingBadgesEventParser : INetworkPacketEventParser
{
    public int PlayerId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        PlayerId = reader.ReadInt();
    }
}