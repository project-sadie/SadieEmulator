using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Events.Parsers.Rooms.Users.Chat;

public class RoomUserChangeChatBubbleEventParser
{
    public ChatBubble Bubble { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Bubble = (ChatBubble) reader.ReadInteger();
    }
}