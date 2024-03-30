using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Handshake;

public class UniqueIdParser
{
    public string MachineId { get; private set; }
    public string Fingerprint { get; private set; }
    public string FlashVersion { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        MachineId = reader.ReadString();
        Fingerprint = reader.ReadString();
        FlashVersion = reader.ReadString();
    }
}