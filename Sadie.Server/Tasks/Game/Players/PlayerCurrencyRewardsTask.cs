using Sadie.Database;
using Sadie.Database.Models.Server;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Writers.Players.Purse;

namespace SadieEmulator.Tasks.Game.Players;

public class PlayerCurrencyRewardsTask(
    SadieContext dbContext,
    IEnumerable<ServerPeriodicCurrencyReward> rewards, 
    PlayerRepository playerRepository,
    ServerSettings serverSettings,
    IRoomUserRepository roomUserRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(1);
    public DateTime LastExecuted { get; set; }

    private readonly Dictionary<int, DateTime> _lastProcessed = rewards
        .ToDictionary(k => k.Id, v => DateTime.Now);
    
    public async Task ExecuteAsync()
    {
        var rewardsToCheck = serverSettings.MakeCurrencyRewardsFair
            ? rewards
            : rewards
                .Where(r => (DateTime.Now - _lastProcessed[r.Id]).TotalSeconds >= r.IntervalSeconds);
        
        foreach (var reward in rewardsToCheck)
        {
            await CheckRewardsForPlayersAsync(reward);
            _lastProcessed[reward.Id] = DateTime.Now;
        }
    }

    private async Task CheckRewardsForPlayersAsync(ServerPeriodicCurrencyReward reward)
    {
        var players = playerRepository.GetAll();
        var logs = new List<ServerPeriodicCurrencyRewardLog>();
        
        foreach (var player in players)
        {
            var failIdleCheck = reward.SkipIdle && 
                roomUserRepository.TryGetById(player.Id, out var roomUser) && 
                roomUser!.IsIdle;
            
            var failRoomCheck = reward.SkipHotelView && 
                player.CurrentRoomId == 0;
         
            if ((serverSettings.MakeCurrencyRewardsFair && 
                 !player.DeservesReward(reward.Type, reward.IntervalSeconds)) ||
                failIdleCheck ||
                failRoomCheck)
            {
                continue;
            }

            await RewardPlayerAsync(player, reward);
            
            logs.Add(new ServerPeriodicCurrencyRewardLog
            {
                PlayerId = player.Id,
                Type = reward.Type,
                Amount = reward.Amount,
                CreatedAt = DateTime.Now
            });
        }

        await dbContext.ServerPeriodicCurrencyRewardLogs.AddRangeAsync(logs);
        await dbContext.SaveChangesAsync();
    }

    private static async Task RewardPlayerAsync(PlayerLogic player, ServerPeriodicCurrencyReward reward)
    {
        switch (reward.Type)
        {
            case "credits":
                player.Data.CreditBalance += reward.Amount;
                await player.NetworkObject!.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(
                    player.Data.PixelBalance,
                    player.Data.SeasonalBalance,
                    player.Data.GotwPoints));
                break;
            case "pixels":
                player.Data.PixelBalance += reward.Amount;
                await player.NetworkObject!.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(
                    player.Data.PixelBalance,
                    player.Data.SeasonalBalance,
                    player.Data.GotwPoints));
                break;
            case "seasonal":
                player.Data.SeasonalBalance += reward.Amount;
                await player.NetworkObject!.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(
                    player.Data.PixelBalance,
                    player.Data.SeasonalBalance,
                    player.Data.GotwPoints));
                break;
        }
    }
}