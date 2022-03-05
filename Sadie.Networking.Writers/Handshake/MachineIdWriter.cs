using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Handshake;

public class MachineIdWriter : NetworkPacketWriter
{
    public MachineIdWriter(string machineId)
    {
        WriteShort(ServerPacketId.UniqueMachineId);
        WriteString(machineId);
    }
}