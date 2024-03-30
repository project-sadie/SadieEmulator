using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Messenger;

public class PlayerStalkParser
{
    public int PlayerId { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        PlayerId = reader.ReadInteger();
    }
}