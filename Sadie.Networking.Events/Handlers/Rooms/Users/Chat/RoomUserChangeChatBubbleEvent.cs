using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

public class RoomUserChangeChatBubbleEvent(RoomUserChangeChatBubbleParser parser) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        client.Player.Data.ChatBubble = parser.Bubble;
    }
}