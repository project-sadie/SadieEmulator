using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerActivityParser
{
    public string Type { get; private set; }
    public string Value { get; private set; }
    public string Action { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Type = reader.ReadString();
        Value = reader.ReadString();
        Action = reader.ReadString();
    }
}