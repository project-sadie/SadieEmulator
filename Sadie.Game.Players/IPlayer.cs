namespace Sadie.Game.Players;

public interface IPlayer : IPlayerData, IDisposable
{
    bool Authenticated { get; set; }
}