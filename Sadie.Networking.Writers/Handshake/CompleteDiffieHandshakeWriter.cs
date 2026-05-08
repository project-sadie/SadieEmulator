using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.CompleteDiffieHandshake)]
public class CompleteDiffieHandshakeWriter : AbstractPacketWriter
{
    public required string PublicKey { get; init; }
    public bool? ClientEncryption { get; init; }
}