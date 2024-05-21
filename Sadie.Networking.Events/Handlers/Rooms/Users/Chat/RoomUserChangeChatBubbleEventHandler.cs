using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerIds.RoomUserChangeChatBubble)]
public class RoomUserChangeChatBubbleEventHandler : INetworkPacketEventHandler
{
    public int Bubble { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        client.Player.AvatarData.ChatBubbleId = (ChatBubble) Bubble;
    }
}