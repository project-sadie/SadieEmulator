using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Codecs;

public class PacketDecoder : MessageToMessageDecoder<IByteBuffer>
{
    protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
    {
        var packetId = message.ReadShort();
        var bytes = message.ReadBytes(message.ReadableBytes);

        output.Add(new NetworkPacket(packetId, bytes.Array));
    }
}