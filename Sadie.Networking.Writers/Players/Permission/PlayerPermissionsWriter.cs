using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Permission;

public class PlayerPermissionsWriter : AbstractPacketWriter
{
    public PlayerPermissionsWriter(int club, int rank, bool ambassador)
    {
        WriteShort(ServerPacketId.PlayerPermissions);
        WriteInteger(club);
        WriteInteger(rank);
        WriteBool(ambassador);
    }
}