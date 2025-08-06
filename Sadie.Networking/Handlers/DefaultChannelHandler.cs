using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Handlers;

public class DefaultChannelHandler(
    ILogger<DefaultChannelHandler> logger,
    INetworkPacketHandler packetHandler,
    INetworkClientRepository clientRepository,
    INetworkClientFactory clientFactory)
    : SimpleChannelInboundHandler<INetworkPacket>
{
    public override void ChannelActive(IChannelHandlerContext context)
    {
        var client = clientFactory.CreateClient(context.Channel);
        clientRepository.AddClient(context.Channel.Id, client);
    }

    public override async void ChannelInactive(IChannelHandlerContext context)
    {
        if (!await clientRepository.TryRemoveAsync(context.Channel.Id))
        {
            logger.LogError("Failed to remove channel from client repository.");
        }
    }

    protected override void ChannelRead0(IChannelHandlerContext context, INetworkPacket packet)
    {
        var client = clientRepository.TryGetClientByChannelId(context.Channel.Id);

        if (client == null)
        {
            return;
        }

        _ = packetHandler.HandleAsync(client, packet);
    }
}