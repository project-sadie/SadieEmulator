namespace Sadie.Networking.Packets.Server.Players.Purse;

internal class PlayerCreditsBalanceWriter : NetworkPacketWriter
{
    internal PlayerCreditsBalanceWriter(long credits) : base(ServerPacketId.PlayerCreditsBalance)
    {
        WriteString(credits + ".0");
    }
}