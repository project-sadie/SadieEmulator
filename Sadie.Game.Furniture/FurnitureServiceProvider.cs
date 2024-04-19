using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Furniture;

public static class FurnitureServiceProvider
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<FurnitureItemRepository>();
    }
}