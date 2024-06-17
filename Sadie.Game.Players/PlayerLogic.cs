using Microsoft.Extensions.Logging;
using Sadie.API.Game.Players;
using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public class PlayerLogic : Player, IPlayerLogic
{
    private readonly ILogger<PlayerLogic> _logger;

    public PlayerLogic(ILogger<PlayerLogic> logger,
        int id,
        string username,
        PlayerData data)
    {
        _logger = logger;
        Id = id;
        Username = username;
        Data = data;
    }

    public INetworkObject? NetworkObject { get; set; }
    public new PlayerData Data { get; }
    public IPlayerState State { get; } = new PlayerState();
    public bool Authenticated { get; set; }
    public int CurrentRoomId { get; set; }

    public ValueTask DisposeAsync()
    {
        _logger.LogInformation($"Player '{Username}' has logged out");
        return ValueTask.CompletedTask;
    }

    public bool DeservesReward(string? rewardType, int intervalInSeconds)
    {
        var lastReward = RewardLogs
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault(x => x.Type == rewardType);

        return lastReward == null || lastReward.CreatedAt < DateTime.Now.AddSeconds(-intervalInSeconds);
    }
}