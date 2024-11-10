using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.CompleteDiffieHandshake)]
public class CompleteDiffieHandshakeWriter : AbstractPacketWriter
{
    public required string PublicKey { get; init; }
    public bool? ClientEncryption { get; init; }
}