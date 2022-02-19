using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Permission;

public class PlayerPermissionsWriter : NetworkPacketWriter
{
    public PlayerPermissionsWriter(int club, int rank, bool ambassador) : base(ServerPacketId.PlayerPermissions)
    {
        WriteInt(club);
        WriteInt(rank);
        WriteBoolean(ambassador);
    }
}