using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events;

public class ClientPacketHandler : INetworkPacketHandler
{
    private readonly ILogger<ClientPacketHandler> _logger;
    private readonly ConcurrentDictionary<int, INetworkPacketEvent> _packets;

    public ClientPacketHandler(ILogger<ClientPacketHandler> logger, ConcurrentDictionary<int, INetworkPacketEvent> packets)
    {
        _logger = logger;
        _packets = packets;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacket packet)
    {
        if (!_packets.TryGetValue(packet.PacketId, out var packetEvent))
        {
            _logger.LogError($"Couldn't resolve packet event for header '{packet.PacketId}'");
            return;
        }

        _logger.LogDebug($"Executing packet '{packetEvent.GetType().Name}'");
        await ExecuteAsync(client, packet, packetEvent);
    }

    private async Task ExecuteAsync(INetworkClient client, INetworkPacket packet, INetworkPacketEvent @event)
    {
        try
        {
            await @event.HandleAsync(client, packet);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
        }
    }
}