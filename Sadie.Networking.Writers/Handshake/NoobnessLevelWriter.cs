using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.NoobnessLevel)]
public class NoobnessLevelWriter : AbstractPacketWriter
{
    public required int Level { get; init; }
}