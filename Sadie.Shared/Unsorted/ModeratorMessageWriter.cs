using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Shared.Unsorted;

[PacketId(ServerPacketId.ModeratorMessage)]
public class ModeratorMessageWriter : AbstractPacketWriter
{
    public required string Message { get; init; }
    public required string Link { get; init; }
}