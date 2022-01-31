namespace Sadie.Networking.Packets.Server.Players.Purse;

public class PlayerCreditsBalance : NetworkPacketWriter
{
    public PlayerCreditsBalance(long credits) : base(ServerPacketIds.PlayerPermissions)
    {
        WriteString(credits + ".0");
    }
}