using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Generic;

public class PlayerRelationshipsEventParser : INetworkPacketEventParser
{
    public int PlayerId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        PlayerId = reader.ReadInteger();
    }
}