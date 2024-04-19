using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.HotelView;

public class HotelViewDataEventParser : INetworkPacketEventParser
{
    public string Unknown1 { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        Unknown1 = reader.ReadString();
    }
}