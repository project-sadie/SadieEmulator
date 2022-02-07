namespace Sadie.Networking.Packets.Server.Players.Purse;

internal class PlayerActivityPointsBalanceWriter : NetworkPacketWriter
{
    internal PlayerActivityPointsBalanceWriter(Dictionary<int, long> currencyMap) : base(ServerPacketId.PlayerActivityPointsBalance)
    {
        WriteInt(currencyMap.Count);

        foreach (var (currency, value) in currencyMap)
        {
            WriteInt(currency);
            WriteLong(value);
        }
    }
}