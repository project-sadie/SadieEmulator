using Microsoft.Extensions.Logging;
using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public class Player : IPlayer
{
    private readonly ILogger<Player> _logger;
    
    public INetworkObject NetworkObject { get; }
    public IPlayerData Data { get; }

    public Player(
        ILogger<Player> logger,
        INetworkObject networkObject,
        IPlayerData data) 
    {
        _logger = logger;
        NetworkObject = networkObject;
        Data = data;
    }
    
    public bool Authenticated { get; set; }

    public ValueTask DisposeAsync()
    {
        _logger.LogInformation($"Player '{Data.Username}' has logged out");
        return ValueTask.CompletedTask;
    }
}