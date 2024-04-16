using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Sadie.Networking;

public class PacketEncoder : MessageToMessageEncoder<byte[]>
{
    protected override void Encode(IChannelHandlerContext context, byte[] message, List<object> output)
    {
        output.Add(message);
    }
}