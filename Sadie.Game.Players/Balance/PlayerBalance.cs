namespace Sadie.Game.Players.Balance;

public class PlayerBalance(long credits, long pixels, long seasonal, long gotw)
    : IPlayerBalance
{
    public long Credits { get; } = credits;
    public long Pixels { get; } = pixels;
    public long Seasonal { get; } = seasonal;
    public long Gotw { get; } = gotw;
}