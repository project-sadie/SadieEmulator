using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Unsorted;

[PacketId(EventHandlerIds.GetCommunityGoalHallOfFame)]
public class GetCommunityGoalHallOfFameEventHandler : INetworkPacketEventHandler
{
    public string Unknown { get; init; }
    
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        throw new NotImplementedException();
    }
}