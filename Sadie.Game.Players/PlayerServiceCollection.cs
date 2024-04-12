using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Players;

public static class PlayerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddTransient<IPlayerState, PlayerState>();
        serviceCollection.AddTransient<PlayerLogic, PlayerLogic>();
        serviceCollection.AddSingleton<PlayerRepository>();
    }
}