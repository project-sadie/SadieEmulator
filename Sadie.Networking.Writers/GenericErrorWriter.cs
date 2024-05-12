using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers;

[PacketId(ServerPacketId.GenericError)]
public class GenericErrorWriter : AbstractPacketWriter
{
    public required int ErrorCode { get; init; }
}