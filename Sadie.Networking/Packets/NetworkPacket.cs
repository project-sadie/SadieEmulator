using Sadie.API.Interfaces.Networking.Packets;

namespace Sadie.Networking.Packets;

public sealed class NetworkPacket(short packetId, byte[] buffer, int offset, int length)
    : INetworkPacket
{
    public short PacketId { get; } = packetId;
    public ReadOnlyMemory<byte> Data { get; } = new(buffer, offset, length);
}