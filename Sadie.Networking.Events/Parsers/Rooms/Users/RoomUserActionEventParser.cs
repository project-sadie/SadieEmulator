using Sadie.Game.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users;

public record RoomUserActionEventParser : INetworkPacketEventParser
{
    public RoomUserAction Action { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Action = (RoomUserAction) reader.ReadInt();
    }
}