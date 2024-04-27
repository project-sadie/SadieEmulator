using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Handshake;

public class UniqueIdWriter : AbstractPacketWriter
{
    public UniqueIdWriter(string machineId)
    {
        WriteShort(ServerPacketId.UniqueId);
        WriteString(machineId);
    }
}