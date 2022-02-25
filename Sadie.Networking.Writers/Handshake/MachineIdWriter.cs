using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Handshake;

public class MachineIdWriter : NetworkPacketWriter
{
    public MachineIdWriter(string machineId) : base(ServerPacketId.UniqueMachineId)
    {
        WriteString(machineId);
    }
}