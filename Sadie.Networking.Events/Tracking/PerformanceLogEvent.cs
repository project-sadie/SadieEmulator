using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Tracking;

public class PerformanceLogEvent : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}