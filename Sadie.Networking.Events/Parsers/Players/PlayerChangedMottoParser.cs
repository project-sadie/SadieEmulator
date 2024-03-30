using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerChangedMottoParser
{
    public string Motto { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Motto = reader.ReadString();
    }
}