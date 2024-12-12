using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Polls;

[PacketId(EventHandlerId.RoomPollVoteCounter)]
public class RoomPollVoteCounterEventHandler : INetworkPacketEventHandler
{
    public int Counter { get; init; }
    
    public Task HandleAsync(INetworkClient client)
    {
        throw new NotImplementedException();
    }
}