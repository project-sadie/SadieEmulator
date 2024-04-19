using Sadie.Database.Models.Server;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Writers.Players.Purse;

namespace SadieEmulator.Tasks.Game.Players;

public class PlayerCurrencyRewardsTask(
    List<ServerPeriodicCurrencyReward> rewards, 
    PlayerRepository playerRepository,
    ServerSettings serverSettings,
    IRoomUserRepository roomUserRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(1);
    public DateTime LastExecuted { get; set; }

    private readonly Dictionary<int, DateTime> _lastProcessed = 
        rewards.ToDictionary(k => k.Id, v => DateTime.Now);
    
    public async Task ExecuteAsync()
    {
        foreach (var reward in rewards.Where(reward => (DateTime.Now - _lastProcessed[reward.Id]).TotalSeconds >= reward.IntervalSeconds))
        {
            await RewardPlayersAsync(reward);
            _lastProcessed[reward.Id] = DateTime.Now;
        }
    }

    private async Task RewardPlayersAsync(ServerPeriodicCurrencyReward reward)
    {
        var players = playerRepository.GetAll();
        
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

            switch (reward.Type)
            {
                case "credits":
                    player.Data.CreditBalance += reward.Amount;
                    await player.NetworkObject!.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(
                        player.Data.PixelBalance,
                        player.Data.SeasonalBalance,
                        player.Data.GotwPoints).GetAllBytes());
                    break;
                case "pixels":
                    player.Data.PixelBalance += reward.Amount;
                    await player.NetworkObject!.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(
                        player.Data.PixelBalance,
                        player.Data.SeasonalBalance,
                        player.Data.GotwPoints).GetAllBytes());
                    break;
                case "seasonal":
                    player.Data.SeasonalBalance += reward.Amount;
                    await player.NetworkObject!.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(
                        player.Data.PixelBalance,
                        player.Data.SeasonalBalance,
                        player.Data.GotwPoints).GetAllBytes());
                    break;
            }
        }
    }
}