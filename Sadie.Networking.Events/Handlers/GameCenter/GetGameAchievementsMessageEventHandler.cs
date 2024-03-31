using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.GameCenter;

public class GetGameAchievementsMessageEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.GetGameAchievementsMessage;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}