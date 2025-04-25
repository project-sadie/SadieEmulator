using System.Net;
using System.Security.Cryptography.X509Certificates;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.Networking.Client;
using Sadie.Networking.Codecs;
using Sadie.Networking.Handlers;
using Sadie.Networking.Packets;
using NetworkOptions = Sadie.Networking.Options.NetworkOptions;
using NetworkPacketOptions = Sadie.Networking.Options.NetworkPacketOptions;

namespace Sadie.Networking
{
    public class NetworkListener(
        ILogger<NetworkListener> logger,
        IOptions<NetworkOptions> options,
        IOptions<NetworkPacketOptions> packetOptions,
        ILogger<DefaultChannelHandler> logger2,
        INetworkPacketHandler packetHandler,
        INetworkClientRepository clientRepository,
        INetworkClientFactory clientFactory)
        : INetworkListener
    {
        private readonly NetworkOptions _networkOptions = options.Value;
        private readonly NetworkPacketOptions _packetOptions = packetOptions.Value;

        private ServerBootstrap _bootstrap;
        private IChannel? _channel;

        public void Bootstrap()
        {
            _bootstrap = new ServerBootstrap()
                .Group(new MultithreadEventLoopGroup(1), new MultithreadEventLoopGroup())
                .Channel<TcpServerSocketChannel>()
                .ChildOption(ChannelOption.TcpNodelay, true)
                .ChildOption(ChannelOption.SoKeepalive, true)
                .ChildOption(ChannelOption.SoRcvbuf, _packetOptions.BufferByteSize)
                .Option(ChannelOption.SoBacklog, 8192)
                .ChildOption(ChannelOption.RcvbufAllocator, new FixedRecvByteBufAllocator(_packetOptions.BufferByteSize))
                .ChildOption(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

                    if (_networkOptions.UseWss &&
                        !string.IsNullOrWhiteSpace(_networkOptions.CertificateFile))
                    {
                        var certificate = new X509Certificate(_networkOptions.CertificateFile);
                        pipeline.AddLast(TlsHandler.Server(certificate));
                    }
                    pipeline.AddLast(new HttpServerCodec());
                    pipeline.AddLast(new HttpObjectAggregator(65536));
                    pipeline.AddLast(new WebSocketServerProtocolHandler("/", null, true, 65536, false, true));
                    pipeline.AddLast(new WebSocketCodec());
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(
                        65536,
                        0,
                        _packetOptions.FrameLengthByteCount,
                        0,
                        _packetOptions.FrameLengthByteCount));

                    pipeline.AddLast(new PacketDecoder());
                    pipeline.AddLast(new PacketEncoder());
                    pipeline.AddLast(new DefaultChannelHandler(logger2, packetHandler, clientRepository, clientFactory));
                }));
        }

        public async Task ListenAsync()
        {
            _channel = await _bootstrap.BindAsync(
                IPAddress.Parse(_networkOptions.Host),
                _networkOptions.Port);

            var address = $"{(_networkOptions.UseWss ? "wss" : "ws")}/{_networkOptions.Host}:{_networkOptions.Port}";

            logger.LogInformation($"Networking is listening for connections on {address}");
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
