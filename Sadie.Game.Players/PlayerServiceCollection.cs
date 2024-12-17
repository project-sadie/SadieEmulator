using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Players;

namespace Sadie.Game.Players;

public static class PlayerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IPlayerState, PlayerState>();
        serviceCollection.AddTransient<PlayerLogic, PlayerLogic>();
        serviceCollection.AddSingleton<IPlayerRepository, PlayerRepository>();
        serviceCollection.AddSingleton<IPlayerHelperService, PlayerHelperService>();
        serviceCollection.AddTransient<IPlayerService, PlayerService>();
    }
}