using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.Db.Models.Players;
using Sadie.Networking.Writers.Players;

namespace Sadie.Game.Players;

public class PlayerLogic : Player, IPlayerLogic
{
    private readonly ILogger<PlayerLogic> _logger;

    public PlayerLogic(ILogger<PlayerLogic> logger,
        long id,
        string username,
        PlayerData data)
    {
        _logger = logger;
        Id = id;
        Username = username;
        Data = data;
    }

    public IChannel? Channel { get; set; }
    public INetworkObject? NetworkObject { get; set; }
    public IPlayerState State { get; } = new PlayerState();
    public bool Authenticated { get; set; }

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

        return lastReward == null ||
               lastReward.CreatedAt < DateTime.Now.AddSeconds(-intervalInSeconds);
    }

    public async Task SendAlertAsync(string message)
    {
        await NetworkObject!.WriteToStreamAsync(new PlayerAlertWriter
        {
            Message = message
        });
    }
}