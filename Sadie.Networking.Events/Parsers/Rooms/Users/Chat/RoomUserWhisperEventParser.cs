using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Events.Parsers.Rooms.Users.Chat;

public class RoomUserWhisperEventParser : INetworkPacketEventParser
{
    public string Data { get; private set; }
    public ChatBubble Bubble { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Data = reader.ReadString();
        Bubble = (ChatBubble) reader.ReadInteger();
    }
}