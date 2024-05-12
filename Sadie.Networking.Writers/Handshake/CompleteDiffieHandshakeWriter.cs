using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.CompleteDiffieHandshake)]
public class CompleteDiffieHandshakeWriter : AbstractPacketWriter
{
    public required string PublicKey { get; init; }
    public bool? ClientEncryption { get; init; }
}