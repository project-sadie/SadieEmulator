using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.NoobnessLevel)]
public class NoobnessLevelWriter : AbstractPacketWriter
{
    public required int Level { get; init; }
}