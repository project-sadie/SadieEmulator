namespace Sadie.Networking.Packets.Server.Players.Purse;

public class PlayerCreditsBalance : NetworkPacketWriter
{
    public PlayerCreditsBalance(long credits) : base(ServerPacketId.PlayerPermissions)
    {
        WriteString(credits + ".0");
    }
}