using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Server;
using Sadie.Game.Players;
using Sadie.Networking.Serialization;
using Sadie.Networking.Writers.Players.Purse;

namespace SadieEmulator.Tasks.Game.Players;

public class PlayerCurrencyRewardsTask(
    SadieContext dbContext,
    List<ServerPeriodicCurrencyReward> rewards, 
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

            var log = new ServerPeriodicCurrencyRewardLog
            {
                PlayerId = player.Id,
                Type = reward.Type,
                Amount = reward.Amount,
                CreatedAt = DateTime.Now
            };
            
            player.RewardLogs.Add(log);
            logs.Add(log);
        }

        await dbContext.ServerPeriodicCurrencyRewardLogs.AddRangeAsync(logs);
        await dbContext.SaveChangesAsync();
    }

    private static async Task RewardPlayerAsync(PlayerLogic player, ServerPeriodicCurrencyReward reward)
    {
        AbstractPacketWriter writer = null;
        
        switch (reward.Type)
        {
            case "credits":
                player.Data.CreditBalance += reward.Amount;
                
                writer = new PlayerCreditsBalanceWriter
                {
                    Credits = player.Data.CreditBalance
                };
                break;
            case "pixels":
                player.Data.PixelBalance += reward.Amount;
                
                writer = new PlayerActivityPointsBalanceWriter
                {
                    PixelBalance = player.Data.PixelBalance,
                    SeasonalBalance = player.Data.SeasonalBalance,
                    GotwPoints = player.Data.GotwPoints
                };
                break;
            case "seasonal":
                player.Data.SeasonalBalance += reward.Amount;
                
                writer = new PlayerActivityPointsBalanceWriter
                {
                    PixelBalance = player.Data.PixelBalance,
                    SeasonalBalance = player.Data.SeasonalBalance,
                    GotwPoints = player.Data.GotwPoints
                };
                break;
        }

        await player.NetworkObject!.WriteToStreamAsync(writer);
    }
}