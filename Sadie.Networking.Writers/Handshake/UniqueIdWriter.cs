using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Handshake;

public class UniqueIdWriter : AbstractPacketWriter
{
    public UniqueIdWriter(string machineId)
    {
        WriteShort(ServerPacketId.UniqueId);
        WriteString(machineId);
    }
}