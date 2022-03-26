namespace Sadie.Game.Players.Balance;

public class PlayerBalance : IPlayerBalance
{
    public long Credits { get; }
    public long Pixels { get; }
    public long Seasonal { get; }
    public long Gotw { get; }
    
    public PlayerBalance(long credits, long pixels, long seasonal, long gotw)
    {
        Credits = credits;
        Pixels = pixels;
        Seasonal = seasonal;
        Gotw = gotw;
    }
}