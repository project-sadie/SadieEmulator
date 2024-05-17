using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerIds.RoomUserChangeChatBubble)]
public class RoomUserChangeChatBubbleEventHandler(RoomUserChangeChatBubbleEventParser eventParser) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        client.Player.AvatarData.ChatBubbleId = eventParser.Bubble;
    }
}