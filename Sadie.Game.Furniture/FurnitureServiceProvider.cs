using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Furniture;

public class FurnitureServiceProvider
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<FurnitureItemDao>();
        serviceCollection.AddSingleton<FurnitureItemFactory>();
        serviceCollection.AddSingleton<FurnitureItem>();
        serviceCollection.AddSingleton<FurnitureItemRepository>();
    }
}