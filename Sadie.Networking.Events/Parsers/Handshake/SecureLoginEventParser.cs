using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Handshake;

public class SecureLoginEventParser : INetworkPacketEventParser
{
    public string? Token { get; private set; }
    public TimeSpan Delay { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Token = reader.ReadString();
        Delay = TimeSpan.FromMilliseconds(reader.ReadInt());
    }
}