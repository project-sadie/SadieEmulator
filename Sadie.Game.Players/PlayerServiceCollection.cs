using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Catalog;
using Sadie.Game.Players.Balance;

namespace Sadie.Game.Players;

public static class PlayerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddTransient<IPlayerState, PlayerState>();
        serviceCollection.AddTransient<IPlayerBalance, PlayerBalance>();
        serviceCollection.AddTransient<PlayerLogic, PlayerLogic>();
        serviceCollection.AddSingleton<IPlayerFactory, PlayerFactory>();
        serviceCollection.AddSingleton<PlayerRepository>();
        
        var playerConstants = new PlayerConstants();
        config.GetSection("Constants:Player").Bind(playerConstants);
        serviceCollection.AddSingleton(playerConstants);
    }
}