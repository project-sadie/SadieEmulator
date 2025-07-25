using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Players;
using Sadie.Game.Players.Options;

namespace Sadie.Game.Players;

public static class PlayerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddTransient<IPlayerState, PlayerState>();
        serviceCollection.AddTransient<PlayerLogic, PlayerLogic>();
        serviceCollection.AddSingleton<IPlayerRepository, PlayerRepository>();
        serviceCollection.AddSingleton<IPlayerHelperService, PlayerHelperService>();
        serviceCollection.AddSingleton<IPlayerLoaderService, PlayerLoaderService>();
        serviceCollection.Configure<PlayerOptions>(options => config.GetSection("PlayerOptions").Bind(options));
    }
}