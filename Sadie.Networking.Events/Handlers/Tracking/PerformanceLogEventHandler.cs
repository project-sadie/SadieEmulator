using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Tracking;

public class PerformanceLogEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PerformanceLog;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}