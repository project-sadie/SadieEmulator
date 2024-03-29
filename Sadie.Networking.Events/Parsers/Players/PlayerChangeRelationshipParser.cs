using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerChangeRelationshipParser
{
    public int PlayerId { get; private set; }
    public int RelationId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        PlayerId = reader.ReadInteger();
        RelationId = reader.ReadInteger();
    }
}