using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game;

public class GameService
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IGameProcessor, GameProcessor>();
    }
}