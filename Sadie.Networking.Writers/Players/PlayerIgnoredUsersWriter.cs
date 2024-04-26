using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerIgnoredUsersWriter : AbstractPacketWriter
{
    public PlayerIgnoredUsersWriter()
    {
        WriteShort(ServerPacketId.PlayerIgnoredUsers);
        WriteInteger(0);
    }
}