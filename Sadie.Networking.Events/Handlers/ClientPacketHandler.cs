using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers;

public class ClientPacketHandler(
    ILogger<ClientPacketHandler> logger,
    ConcurrentDictionary<int, INetworkPacketEvent> packets)
    : INetworkPacketHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacket packet)
    {
        if (!packets.TryGetValue(packet.PacketId, out var packetEvent))
        {
            logger.LogError($"Couldn't resolve packet event for header '{packet.PacketId}'");
            return;
        }

        await ExecuteAsync(client, packet, packetEvent);
    }

    private async Task ExecuteAsync(INetworkClient client, INetworkPacketReader packet, INetworkPacketEvent @event)
    {
        logger.LogDebug($"Executing packet '{@event.GetType().Name}'");
        
        try
        {
            await @event.HandleAsync(client, packet);
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }
}