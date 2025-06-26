using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Polls;

[PacketId(EventHandlerId.RoomPollStart)]
public class RoomPollStartEventHandler : INetworkPacketEventHandler
{
    public int Unknown { get; init; }
    
    public Task HandleAsync(INetworkClient client)
    {
        throw new NotImplementedException();
    }
}