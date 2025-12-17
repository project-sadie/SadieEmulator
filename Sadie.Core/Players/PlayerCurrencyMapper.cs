namespace Sadie.Core.Players;

public static class PlayerCurrencyMapper
{
    public static Dictionary<int, long> FromBalances(
        long pixelBalance,
        long seasonalBalance,
        long gotwPoints)
    {
        return new Dictionary<int, long>
        {
            { 0, pixelBalance },
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, seasonalBalance },
            { 101, 0 },
            { 102, 0 },
            { 103, gotwPoints },
            { 104, 0 },
            { 105, 0 }
        };
    }
}