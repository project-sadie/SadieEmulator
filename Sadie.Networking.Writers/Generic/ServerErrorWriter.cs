using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Generic;

public class ServerErrorWriter : AbstractPacketWriter
{
    public required int MessageId { get; init; }
    public required int ErrorCode { get; init; }
    public required string DateTime { get; init; }
}