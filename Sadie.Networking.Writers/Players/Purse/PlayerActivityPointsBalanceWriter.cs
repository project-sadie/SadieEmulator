using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Purse;

public class PlayerActivityPointsBalanceWriter : NetworkPacketWriter
{
    public PlayerActivityPointsBalanceWriter(int pixelBalance, int seasonalBalance, int gotwPoints)
    {
        var currencies = new Dictionary<int, long>
        {
            {0, pixelBalance},
            {1, 0}, // snowflakes
            {2, 0}, // hearts
            {3, 0}, // gift points
            {4, 0}, // shells
            {5, seasonalBalance},
            {101, 0}, // snowflakes
            {102, 0}, // unknown
            {103, gotwPoints},
            {104, 0}, // unknown
            {105, 0} // unknown
        };
        
        WriteShort(ServerPacketId.PlayerActivityPointsBalance);
        WriteInteger(currencies.Count);

        foreach (var (currency, value) in currencies)
        {
            WriteInteger(currency);
            WriteLong(value);
        }
    }
}