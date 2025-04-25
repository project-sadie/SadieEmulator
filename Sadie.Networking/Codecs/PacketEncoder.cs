using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Sadie.Networking.Serialization;

namespace Sadie.Networking.Codecs;

public class PacketEncoder : MessageToByteEncoder<NetworkPacketWriter>
{
    protected override void Encode(IChannelHandlerContext context, NetworkPacketWriter message, IByteBuffer output)
    {
        output.WriteBytes(message.GetAllBytes());
    }
}