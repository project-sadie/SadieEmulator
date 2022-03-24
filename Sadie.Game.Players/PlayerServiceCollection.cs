using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Players.Friendships;

namespace Sadie.Game.Players;

public class PlayerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddTransient<IPlayerBalance, PlayerBalance>();
        serviceCollection.AddTransient<IPlayer, Player>();
        serviceCollection.AddSingleton<IPlayerFactory, PlayerFactory>();
        serviceCollection.AddSingleton<IPlayerDao, PlayerDao>();
        serviceCollection.AddSingleton<IPlayerRepository, PlayerRepository>();
        serviceCollection.AddSingleton<IPlayerFriendshipDao, PlayerFriendshipDao>();
        serviceCollection.AddSingleton<IPlayerFriendshipRepository, PlayerFriendshipRepository>();
        
        var playerConstants = new PlayerConstants();
        config.GetSection("Constants:Player").Bind(playerConstants);
        serviceCollection.AddSingleton(playerConstants);
    }
}