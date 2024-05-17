using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Generic;

namespace Sadie.Networking.Events.Handlers;

public class ClientPacketHandler(
    ILogger<ClientPacketHandler> logger,
    Dictionary<short, Type> packetHandlerTypeMap,
    IServiceProvider serviceProvider)
    : INetworkPacketHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacket packet)
    {
        if (!packetHandlerTypeMap.TryGetValue(packet.PacketId, out var packetEventType))
        {
            var writer = new ServerErrorWriter
            {
                MessageId = packet.PacketId,
                ErrorCode = 1,
                DateTime = DateTime.Now.ToString("M/d/yy, h:mm tt")
            };
                
            await client.WriteToStreamAsync(writer);
            logger.LogWarning($"Couldn't resolve packet eventHandler for header '{packet.PacketId}'");
            return;
        }

        var eventHandler = (INetworkPacketEventHandler) ActivatorUtilities.CreateInstance(serviceProvider, packetEventType);
        await ExecuteAsync(client, packet, eventHandler);
    }

    private async Task ExecuteAsync(INetworkClient client, INetworkPacketReader packet, INetworkPacketEventHandler eventHandler)
    {
        logger.LogDebug($"Executing packet '{eventHandler.GetType().Name}'");
        
        try
        {
            await eventHandler.HandleAsync(client, packet);
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }
}