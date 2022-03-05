using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerIgnoredUsersWriter : NetworkPacketWriter
{
    public PlayerIgnoredUsersWriter()
    {
        WriteShort(ServerPacketId.PlayerIgnoredUsers);
        WriteInt(0);
    }
}