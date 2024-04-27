using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Shared.Unsorted;

[PacketId(ServerPacketId.ModeratorMessage)]
public class ModeratorMessageWriter : AbstractPacketWriter
{
    public required string Message { get; init; }
    public required string Link { get; init; }
}