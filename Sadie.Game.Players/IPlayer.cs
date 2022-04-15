using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public interface IPlayer : IAsyncDisposable
{
    INetworkObject NetworkObject { get; }
    bool Authenticated { get; set; }
    IPlayerData Data { get; }
    IPlayerState State { get; }
}