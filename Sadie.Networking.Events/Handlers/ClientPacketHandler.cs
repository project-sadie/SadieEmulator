using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Generic;

namespace Sadie.Networking.Events.Handlers;

public class ClientPacketHandler : INetworkPacketHandler
{
    private readonly ILogger<ClientPacketHandler> _logger;
    private readonly Dictionary<int, INetworkPacketEventHandler> _packets;

    public ClientPacketHandler(
        ILogger<ClientPacketHandler> logger, 
        IEnumerable<INetworkPacketEventHandler> packetHandlers)
    {
        _logger = logger;
        _packets = packetHandlers.ToDictionary(i => i.Id, h => h);
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacket packet)
    {
        if (!_packets.TryGetValue(packet.PacketId, out var packetEvent))
        {
            await client.WriteToStreamAsync(new ServerErrorWriter(packet.PacketId, 1));
            _logger.LogWarning($"Couldn't resolve packet eventHandler for header '{packet.PacketId}'");
            return;
        }

        await ExecuteAsync(client, packet, packetEvent);
    }

    private async Task ExecuteAsync(INetworkClient client, INetworkPacketReader packet, INetworkPacketEventHandler eventHandler)
    {
        _logger.LogDebug($"Executing packet '{eventHandler.GetType().Name}'");
        
        try
        {
            await eventHandler.HandleAsync(client, packet);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
        }
    }
}