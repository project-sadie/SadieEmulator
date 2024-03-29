using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Handshake;

public class MachineIdParser
{
    public string MachineId { get; private set; }
    public string Fingerprint { get; private set; }
    public string Unknown1 { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        MachineId = reader.ReadString();
        Fingerprint = reader.ReadString();
        Unknown1 = reader.ReadString();
    }
}