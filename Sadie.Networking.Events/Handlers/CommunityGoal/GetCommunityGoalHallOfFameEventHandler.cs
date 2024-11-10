using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.CommunityGoal;

[PacketId(EventHandlerId.GetCommunityGoalHallOfFame)]
public class GetCommunityGoalHallOfFameEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client)
    {
        return Task.CompletedTask;
    }
}