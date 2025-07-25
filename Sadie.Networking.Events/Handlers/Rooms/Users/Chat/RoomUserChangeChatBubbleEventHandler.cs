using Sadie.Enums.Miscellaneous;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerId.RoomUserChangeChatBubble)]
public class RoomUserChangeChatBubbleEventHandler : INetworkPacketEventHandler
{
    public int Bubble { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        client.Player.AvatarData.ChatBubbleId = (ChatBubble) Bubble;
    }
}