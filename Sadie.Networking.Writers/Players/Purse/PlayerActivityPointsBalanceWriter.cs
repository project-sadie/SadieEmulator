namespace Sadie.Networking.Packets.Server.Players.Purse;

public class PlayerActivityPointsBalanceWriter : NetworkPacketWriter
{
    public PlayerActivityPointsBalanceWriter(Dictionary<int, long> currencyMap) : base(ServerPacketId.PlayerActivityPointsBalance)
    {
        WriteInt(currencyMap.Count);

        foreach (var (currency, value) in currencyMap)
        {
            WriteInt(currency);
            WriteLong(value);
        }
    }
}