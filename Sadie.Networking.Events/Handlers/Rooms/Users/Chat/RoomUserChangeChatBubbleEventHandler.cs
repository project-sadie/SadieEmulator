using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

public class RoomUserChangeChatBubbleEventHandler(RoomUserChangeChatBubbleEventParser eventParser) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserChangeChatBubble;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        client.Player.Data.ChatBubble = eventParser.Bubble;
    }
}