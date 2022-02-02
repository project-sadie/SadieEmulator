namespace Sadie.Networking.Packets.Server.Players.Permission;

public class PlayerPermissions : NetworkPacketWriter
{
    public PlayerPermissions() : base(ServerPacketId.PlayerPermissions)
    {
        WriteInt(1); // habbo club
        WriteInt(1); // rank
        WriteBoolean(true); // ambassador
    }
}