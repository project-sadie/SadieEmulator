using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Purse;

public class PlayerCreditsBalanceWriter : AbstractPacketWriter
{
    public PlayerCreditsBalanceWriter(long credits)
    {
        WriteShort(ServerPacketId.PlayerCreditsBalance);
        WriteString(credits + ".0");
    }
}