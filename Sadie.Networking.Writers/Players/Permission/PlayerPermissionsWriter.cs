using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Permission;

public class PlayerPermissionsWriter : NetworkPacketWriter
{
    public PlayerPermissionsWriter(int club, int rank, bool ambassador)
    {
        WriteShort(ServerPacketId.PlayerPermissions);
        WriteInt(club);
        WriteInt(rank);
        WriteBoolean(ambassador);
    }
}