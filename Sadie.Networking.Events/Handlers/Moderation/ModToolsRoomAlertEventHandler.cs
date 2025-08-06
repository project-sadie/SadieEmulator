using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Moderation;

[PacketId(EventHandlerId.ModToolsRoomAlert)]
public class ModToolsRoomAlertEventHandler : INetworkPacketEventHandler
{
    public int Type { get; set; }
    public required string Message { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        await client.RoomUser?.Room.UserRepository?.BroadcastDataAsync(new PlayerAlertWriter
        {
            Message = Message
        })!;
    }
}