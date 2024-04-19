using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Club;

public class PlayerClubOffersEventParser : INetworkPacketEventParser
{
    public int WindowId { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        WindowId = reader.ReadInt();
    }
}
