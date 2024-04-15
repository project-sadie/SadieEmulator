using System.Text;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;

namespace Sadie.Networking;

public class WebSocketServerHandler(bool secure) : SimpleChannelInboundHandler<object>
{
    private WebSocketServerHandshaker? _handshaker;

    protected override void ChannelRead0(IChannelHandlerContext context, object message)
    {
        switch (message)
        {
            case IFullHttpRequest request:
                HandleHttpRequest(context, request);
                break;
            case WebSocketFrame frame:
                HandleWebSocketFrame(context, frame);
                break;
        }
    }
    
    private void HandleHttpRequest(IChannelHandlerContext context, IFullHttpRequest request)
    {
        var wsFactory = new WebSocketServerHandshakerFactory(
            GetWebSocketLocation(request), 
            null, 
            true, 
            5 * 1024 * 1024);

        _handshaker = wsFactory.NewHandshaker(request);
        _handshaker?.HandshakeAsync(context.Channel, request);
    }
    
    private void HandleWebSocketFrame(IChannelHandlerContext context, WebSocketFrame frame)
    {
        switch (frame)
        {
            case CloseWebSocketFrame:
                _handshaker?.CloseAsync(context.Channel, (CloseWebSocketFrame)frame.Retain());
                return;
            case PingWebSocketFrame:
                return;
            case BinaryWebSocketFrame:
                Console.WriteLine($"{Encoding.Default.GetString(frame.Content.Array)}");
                break;
        }
    }

    private string GetWebSocketLocation(IHttpMessage request)
    {
        request.Headers.TryGet(HttpHeaderNames.Host, out var value);
        return (secure ? "wss" : "ws") + value.ToString();
    }
}