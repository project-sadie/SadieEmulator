using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Handshake;

public class CompleteDiffieHandshakeEventParser : INetworkPacketEventParser
{
    public string? PublicKey { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        PublicKey = reader.ReadString();
    }
}