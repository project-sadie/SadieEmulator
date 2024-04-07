using Microsoft.Extensions.Logging;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public class PlayerLogic(
    ILogger<PlayerLogic> logger,
    INetworkObject networkObject,
    PlayerData data)
    : IPlayer
{
    public INetworkObject NetworkObject { get; } = networkObject;
    public PlayerData Data { get; } = data;
    public IPlayerState State { get; } = new PlayerState();

    public bool Authenticated { get; set; }

    public ValueTask DisposeAsync()
    {
        logger.LogInformation($"Player '{Data.Username}' has logged out");
        return ValueTask.CompletedTask;
    }
}