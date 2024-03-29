using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users;

public record RoomUserRespectParser
{
    public int TargetId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        TargetId = reader.ReadInteger();
    }
}