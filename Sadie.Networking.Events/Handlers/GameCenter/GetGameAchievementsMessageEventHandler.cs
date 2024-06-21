using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.GameCenter;

[PacketId(EventHandlerId.GetGameAchievementsMessage)]
public class GetGameAchievementsMessageEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client)
    {
        return Task.CompletedTask;
    }
}