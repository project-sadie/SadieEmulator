using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users;

public class RoomUserTagsEventParser : INetworkPacketEventParser
{
    public int UserId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        UserId = reader.ReadInteger();
    }
}