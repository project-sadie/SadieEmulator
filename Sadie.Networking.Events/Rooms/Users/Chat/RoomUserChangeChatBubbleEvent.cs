using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Shared.Game;

namespace Sadie.Networking.Events.Rooms.Users.Chat;

public class RoomUserChangeChatBubbleEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        client.Player.Data.ChatBubble = (ChatBubble) reader.ReadInteger();
    }
}