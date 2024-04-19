using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users;

public record RoomUserDanceEventParser : INetworkPacketEventParser
{
    public int DanceId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        DanceId = reader.ReadInt();
    }
}