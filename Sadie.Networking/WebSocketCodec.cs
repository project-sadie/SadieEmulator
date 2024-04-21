using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;

namespace Sadie.Networking;

public class WebSocketCodec : MessageToMessageCodec<WebSocketFrame, IByteBuffer>
{
    protected override void Encode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
    {
        output.Add(new BinaryWebSocketFrame(message).Retain());
    }

    protected override void Decode(IChannelHandlerContext context, WebSocketFrame message, List<object> output)
    {
        output.Add(message.Content.Retain());
    }
}