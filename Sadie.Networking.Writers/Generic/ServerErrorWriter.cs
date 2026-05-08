using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Generic;

[PacketId(ServerPacketId.ServerError)]
public class ServerErrorWriter : AbstractPacketWriter
{
    public required int MessageId { get; init; }
    public required int ErrorCode { get; init; }
    public required string DateTime { get; init; }
}