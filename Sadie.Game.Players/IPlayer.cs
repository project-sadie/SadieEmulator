namespace Sadie.Game.Players;

public interface IPlayer : IPlayerData, IAsyncDisposable
{
    bool Authenticated { get; set; }
    bool HasPermission(string name);
}