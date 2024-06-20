using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Unsorted;

namespace Sadie.Networking.Events.Handlers.Unsorted;

[PacketId(EventHandlerIds.GetCommunityGoalHallOfFame)]
public class GetCommunityGoalHallOfFameEventHandler : INetworkPacketEventHandler
{
    public string K { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new CommunityGoalHallOfFameWriter
        {
            GoalCode = "",
            Data = []
        });
    }
}