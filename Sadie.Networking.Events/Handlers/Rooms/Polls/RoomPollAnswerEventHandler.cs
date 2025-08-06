using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Polls;

[PacketId(EventHandlerId.RoomPollAnswer)]
public class RoomPollAnswerEventHandler : INetworkPacketEventHandler
{
    public required int PollId { get; set; }
    public required int QuestionId { get; set; }
    public required List<string> Answers { get; set; }
    
    public Task HandleAsync(INetworkClient client)
    {
        throw new NotImplementedException();
    }
}