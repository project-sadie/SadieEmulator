using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.UniqueId)]
public class UniqueIdWriter : AbstractPacketWriter
{
    public required string MachineId { get; set; }
}