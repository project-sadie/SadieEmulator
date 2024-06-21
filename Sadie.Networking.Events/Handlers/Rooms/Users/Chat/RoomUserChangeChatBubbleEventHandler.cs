using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerId.RoomUserChangeChatBubble)]
public class RoomUserChangeChatBubbleEventHandler : INetworkPacketEventHandler
{
    public int Bubble { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        client.Player.AvatarData.ChatBubbleId = (ChatBubble) Bubble;
    }
}