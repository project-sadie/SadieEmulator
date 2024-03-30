using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users;

public class RoomUserSignParser
{
    public int SignId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        SignId = reader.ReadInteger();
    }
}