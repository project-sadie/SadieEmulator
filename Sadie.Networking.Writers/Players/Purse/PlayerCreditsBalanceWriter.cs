using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Purse;

public class PlayerCreditsBalanceWriter : NetworkPacketWriter
{
    public PlayerCreditsBalanceWriter(long credits) : base(ServerPacketId.PlayerCreditsBalance)
    {
        WriteString(credits + ".0");
    }
}