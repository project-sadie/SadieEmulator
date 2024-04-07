using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public interface IPlayer : IAsyncDisposable
{
    INetworkObject NetworkObject { get; }
    bool Authenticated { get; set; }
    PlayerData Data { get; }
    IPlayerState State { get; }
}