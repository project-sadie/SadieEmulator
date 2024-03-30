using Sadie.Game.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users;

public record RoomUserActionParser
{
    public RoomUserAction Action { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Action = (RoomUserAction) reader.ReadInteger();
    }
}