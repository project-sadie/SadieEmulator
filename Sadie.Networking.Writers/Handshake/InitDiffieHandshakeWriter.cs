using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.InitDiffieHandshake)]
public class InitDiffieHandshakeWriter : AbstractPacketWriter
{
    public required string SingedPrime { get; init; }
    public required string SignedGenerator { get; init; }
}