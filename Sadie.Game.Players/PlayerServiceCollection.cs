using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Respect;
using Sadie.Game.Players.Subscriptions;

namespace Sadie.Game.Players;

public class PlayerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton<IPlayerSubscription, PlayerSubscription>();
        serviceCollection.AddSingleton<IPlayerSubscriptionFactory, PlayerSubscriptionFactory>();
        serviceCollection.AddSingleton<IPlayerSubscriptionDao, PlayerSubscriptionDao>();
        serviceCollection.AddSingleton<IPlayerRespectDao, PlayerRespectDao>();
        serviceCollection.AddSingleton<IPlayerBadgeDao, PlayerBadgeDao>();
        serviceCollection.AddSingleton<IPlayerBadgeFactory, PlayerBadgeFactory>();
        serviceCollection.AddSingleton<IPlayerBadgeRepository, PlayerBadgeRepository>();
        serviceCollection.AddTransient<IPlayerBalance, PlayerBalance>();
        serviceCollection.AddTransient<IPlayer, Player>();
        serviceCollection.AddTransient<IPlayerData, PlayerData>();
        serviceCollection.AddSingleton<IPlayerDataFactory, PlayerDataFactory>();
        serviceCollection.AddSingleton<IPlayerDataDao, PlayerDataDao>();
        serviceCollection.AddSingleton<IPlayerFactory, PlayerFactory>();
        serviceCollection.AddSingleton<IPlayerDao, PlayerDao>();
        serviceCollection.AddSingleton<IPlayerRepository, PlayerRepository>();
        serviceCollection.AddSingleton<IPlayerFriendshipDao, PlayerFriendshipDao>();
        serviceCollection.AddSingleton<IPlayerFriendshipRepository, PlayerFriendshipRepository>();
        serviceCollection.AddSingleton<PlayerFriendshipFactory>();
        
        var playerConstants = new PlayerConstants();
        config.GetSection("Constants:Player").Bind(playerConstants);
        serviceCollection.AddSingleton(playerConstants);
    }
}