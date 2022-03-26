namespace Sadie.Game.Players.Balance;

public interface IPlayerBalance
{
    long Credits { get; }
    long Pixels { get; }
    long Seasonal { get; }
    long Gotw { get; }
}