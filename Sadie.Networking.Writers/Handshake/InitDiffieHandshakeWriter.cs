using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.InitDiffieHandshake)]
public class InitDiffieHandshakeWriter : AbstractPacketWriter
{
    public required string SingedPrime { get; init; }
    public required string SignedGenerator { get; init; }
}