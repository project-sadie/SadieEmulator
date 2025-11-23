using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerId.RoomUserChangeChatBubble)]
public class RoomUserChangeChatBubbleEventHandler : INetworkPacketEventHandler
{
    public int Bubble { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        client.Player.Player.AvatarData.ChatBubbleId = (ChatBubble) Bubble;
    }
}