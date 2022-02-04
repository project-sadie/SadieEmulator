namespace Sadie.Networking.Packets.Server.Players.Purse;

public class PlayerCreditsBalanceWriter : NetworkPacketWriter
{
    public PlayerCreditsBalanceWriter(long credits) : base(ServerPacketId.PlayerCreditsBalance)
    {
        WriteString(credits + ".0");
    }
}