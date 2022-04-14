using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Purse;

public class PlayerActivityPointsBalanceWriter : NetworkPacketWriter
{
    public PlayerActivityPointsBalanceWriter(Dictionary<int, long> currencyMap)
    {
        WriteShort(ServerPacketId.PlayerActivityPointsBalance);
        WriteInteger(currencyMap.Count);

        foreach (var (currency, value) in currencyMap)
        {
            WriteInteger(currency);
            WriteLong(value);
        }
    }
}