using System.Buffers.Binary;
using Sadie.API.Interfaces.Networking.Packets;

namespace Sadie.Networking.Packets;

public class NetworkPacketDecoder : INetworkPacketDecoder
{
    public INetworkPacket Decode(Guid guid, byte[] buffer, int length)
    {
        var span = buffer.AsSpan(0, length);

        var offset = 0;

        _ = BinaryPrimitives.ReadInt32BigEndian(span.Slice(offset, 4));
        offset += 4;

        var packetId = BinaryPrimitives.ReadInt16BigEndian(span.Slice(offset, 2));
        offset += 2;

        var bodyOffset = offset;
        var bodyLength = length - offset;

        return new NetworkPacket(packetId, buffer, bodyOffset, bodyLength);
    }
}