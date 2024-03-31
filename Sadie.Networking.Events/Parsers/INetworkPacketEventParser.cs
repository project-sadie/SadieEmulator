using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers;

public interface INetworkPacketEventParser
{
    void Parse(INetworkPacketReader reader);
}