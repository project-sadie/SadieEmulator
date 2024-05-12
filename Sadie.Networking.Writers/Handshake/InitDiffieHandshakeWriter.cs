using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.InitDiffieHandshake)]
public class InitDiffieHandshakeWriter : AbstractPacketWriter
{
    public required string SingedPrime { get; init; }
    public required string SignedGenerator { get; init; }
}