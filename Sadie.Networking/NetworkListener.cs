using System.Net;
using System.Security.Cryptography.X509Certificates;
using DotNetty.Codecs.Http;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;

namespace Sadie.Networking
{
    public class NetworkListener(
        ILogger<NetworkListener> logger,
        X509Certificate? certificate)
        : INetworkListener
    {
        private IChannel? _channel;

        public async Task ListenAsync(int port)
        {
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workGroup = new MultithreadEventLoopGroup();
            
            var bootstrap = new ServerBootstrap();
            
            bootstrap.Group(bossGroup, workGroup);
            bootstrap.Channel<TcpServerSocketChannel>();
            
            bootstrap
                .Option(ChannelOption.SoBacklog, 8192)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

                    if (certificate != null)
                    {
                        pipeline.AddLast(TlsHandler.Server(certificate));
                    }
                    
                    pipeline.AddLast(new HttpServerCodec());
                    pipeline.AddLast(new HttpObjectAggregator(65536));
                    pipeline.AddLast(new WebSocketServerHandler(certificate == null));
                }));
            
            _channel = await bootstrap.BindAsync(IPAddress.Any, port);
            
            logger.LogInformation("Networking is listening for connections");
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
            }
        }
    }
}
