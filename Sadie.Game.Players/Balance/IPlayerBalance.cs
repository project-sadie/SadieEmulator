namespace Sadie.Game.Players.Balance;

public interface IPlayerBalance
{
    long Credits { get; set; }
    long Pixels { get; set; }
    long Seasonal { get; set; }
    long Gotw { get; }
}