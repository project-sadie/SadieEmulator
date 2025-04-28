using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Packets;

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
        try
        {
            if (!await clientRepository.TryRemoveAsync(context.Channel.Id))
            {
                logger.LogError("Failed to remove channel from client repository.");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }

    protected override async void ChannelRead0(IChannelHandlerContext context, INetworkPacket packet)
    {
        try
        {
            var client = clientRepository.TryGetClientByChannelId(context.Channel.Id);

            if (client == null)
            {
                return;
            }

            await packetHandler.HandleAsync(client, packet);
        }
        catch (IndexOutOfRangeException e)
        {
            logger.LogError($"Failed to handle packet: {packet.GetType().BaseType?.Name}");
            logger.LogError(e.ToString());
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }
}