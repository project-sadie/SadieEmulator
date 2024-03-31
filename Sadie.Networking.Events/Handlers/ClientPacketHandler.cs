using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers;

public class ClientPacketHandler(
    ILogger<ClientPacketHandler> logger,
    ConcurrentDictionary<int, INetworkPacketEventHandler> packets)
    : INetworkPacketHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacket packet)
    {
        if (!packets.TryGetValue(packet.PacketId, out var packetEvent))
        {
            logger.LogError($"Couldn't resolve packet eventHandler for header '{packet.PacketId}'");
            return;
        }

        await ExecuteAsync(client, packet, packetEvent);
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