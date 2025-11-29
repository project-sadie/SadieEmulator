using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sadie.Game.Mappers;

public static class MapperServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        var profiles = new[]
        {
            typeof(RoomProfile),
            typeof(PlayerProfile),
            typeof(NavigatorProfile),
            typeof(CatalogProfile),
            typeof(FurnitureItemProfile)
        };
        
        foreach (var profile in profiles)
        {
            serviceCollection.AddSingleton(profile);
        }

        serviceCollection.AddSingleton(provider => new MapperConfiguration(c =>
        {
            foreach (var profile in profiles)
            {
                c.AddProfile(provider.GetRequiredService(profile) as Profile);
            }

            c.ShouldMapProperty = p => p.GetIndexParameters().Length == 0;
        }, provider.GetRequiredService<ILoggerFactory>()).CreateMapper());
    }
}