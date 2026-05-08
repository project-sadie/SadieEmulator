using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.UniqueId)]
public class UniqueIdWriter : AbstractPacketWriter
{
    public required string MachineId { get; set; }
}