using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Events.Parsers.Rooms.Users.Chat;

public class RoomUserChatEventParser : INetworkPacketEventParser
{
    public string Message { get; private set; }
    public ChatBubble Bubble { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Message = reader.ReadString();
        Bubble = (ChatBubble) reader.ReadInt();
    }
}