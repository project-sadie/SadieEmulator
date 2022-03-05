using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Purse;

public class PlayerCreditsBalanceWriter : NetworkPacketWriter
{
    public PlayerCreditsBalanceWriter(long credits)
    {
        WriteShort(ServerPacketId.PlayerCreditsBalance);
        WriteString(credits + ".0");
    }
}