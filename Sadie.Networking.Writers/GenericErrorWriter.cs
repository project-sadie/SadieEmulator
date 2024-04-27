using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers;

[PacketId(ServerPacketId.GenericError)]
public class GenericErrorWriter : NetworkPacketWriter
{
    public required int ErrorCode { get; init; }
}