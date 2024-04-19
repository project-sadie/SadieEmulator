using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users;

public record RoomUserRespectEventParser : INetworkPacketEventParser
{
    public int TargetId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        TargetId = reader.ReadInt();
    }
}