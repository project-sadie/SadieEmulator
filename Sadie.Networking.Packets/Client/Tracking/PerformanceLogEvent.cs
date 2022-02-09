using Sadie.Networking.Client;

namespace Sadie.Networking.Packets.Client.Tracking;

public class PerformanceLogEvent : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}