using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Doorbell;

public class RoomDoorbellAcceptedEventParser : INetworkPacketEventParser
{
    public int RoomId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        RoomId = reader.ReadInt();
    }
}