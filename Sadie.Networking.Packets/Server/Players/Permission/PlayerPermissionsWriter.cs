namespace Sadie.Networking.Packets.Server.Players.Permission;

internal class PlayerPermissionsWriter : NetworkPacketWriter
{
    internal PlayerPermissionsWriter(int club, int rank, bool ambassador) : base(ServerPacketId.PlayerPermissions)
    {
        WriteInt(club);
        WriteInt(rank);
        WriteBoolean(ambassador);
    }
}