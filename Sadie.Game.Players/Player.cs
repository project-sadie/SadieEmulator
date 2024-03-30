using Microsoft.Extensions.Logging;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public class Player(
    ILogger<Player> logger,
    INetworkObject networkObject,
    IPlayerData data)
    : IPlayer
{
    public INetworkObject NetworkObject { get; } = networkObject;
    public IPlayerData Data { get; } = data;
    public IPlayerState State { get; } = new PlayerState();

    public bool Authenticated { get; set; }

    public ValueTask DisposeAsync()
    {
        logger.LogInformation($"Player '{Data.Username}' has logged out");
        return ValueTask.CompletedTask;
    }
}