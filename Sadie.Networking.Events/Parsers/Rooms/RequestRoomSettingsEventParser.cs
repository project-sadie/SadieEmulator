using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms;

public class RequestRoomSettingsEventParser : INetworkPacketEventParser
{
    public int RoomId { get; private set; }
    public void Parse(INetworkPacketReader reader)
    {
        RoomId = reader.ReadInt();
    }
}