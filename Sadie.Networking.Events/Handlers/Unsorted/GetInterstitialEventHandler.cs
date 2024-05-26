using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Unsorted;

public class GetInterstitialEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.GetInterstitial;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}