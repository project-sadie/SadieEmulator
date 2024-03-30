using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Handshake;

public class MachineIdWriter : NetworkPacketWriter
{
    public MachineIdWriter(string machineId)
    {
        WriteShort(ServerPacketId.UniqueMachineId);
        WriteString(machineId);
    }
}