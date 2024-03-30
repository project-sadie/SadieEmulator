using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Handshake;

public class SecureLoginParser
{
    public string Token { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Token = reader.ReadString();
    }
}