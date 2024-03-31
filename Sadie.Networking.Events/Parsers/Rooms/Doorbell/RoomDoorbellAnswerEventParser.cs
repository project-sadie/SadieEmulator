using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Doorbell;

public class RoomDoorbellAnswerEventParser : INetworkPacketEventParser
{
    public string Username { get; private set; }
    public bool Accept { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Username = reader.ReadString();
        Accept = reader.ReadBool();
    }
}