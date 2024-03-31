using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users.Chat;

public class RoomUserWhisperEventParser : INetworkPacketEventParser
{
    public string Data { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Data = reader.ReadString();
    }
}