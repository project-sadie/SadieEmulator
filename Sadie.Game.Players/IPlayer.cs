using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public interface IPlayer : IPlayerData, IAsyncDisposable
{
    INetworkObject NetworkObject { get; }
    bool Authenticated { get; set; }
    bool HasPermission(string name);
}