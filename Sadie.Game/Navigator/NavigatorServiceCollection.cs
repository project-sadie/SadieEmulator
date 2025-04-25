using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Navigator;
using Sadie.Game.Navigator.Filterers;

namespace Sadie.Game.Navigator;

public static class NavigatorServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        serviceCollection.AddTransient<INavigatorRoomProvider, NavigatorRoomProvider>();
        
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<INavigatorSearchFilterer>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }
}