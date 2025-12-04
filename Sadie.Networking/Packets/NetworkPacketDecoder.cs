using System.Buffers.Binary;
using Sadie.API.Interfaces.Networking.Packets;

namespace Sadie.Networking.Packets;

public class NetworkPacketDecoder : INetworkPacketDecoder
{
    public INetworkPacket Decode(Guid guid, ReadOnlySpan<byte> data)
    {
        var offset = 0;

        _ = BinaryPrimitives.ReadInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        var packetId = BinaryPrimitives.ReadInt16BigEndian(data.Slice(offset, 2));
        offset += 2;
            
        var body = data[offset..].ToArray();
        return new NetworkPacket(packetId, body);
    }
}