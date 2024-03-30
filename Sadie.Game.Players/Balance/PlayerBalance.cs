namespace Sadie.Game.Players.Balance;

public class PlayerBalance(long credits, long pixels, long seasonal, long gotw)
    : IPlayerBalance
{
    public long Credits { get; set; } = credits;
    public long Pixels { get; set; } = pixels;
    public long Seasonal { get; set; } = seasonal;
    public long Gotw { get; } = gotw;
}