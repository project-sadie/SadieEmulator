using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Database.Mappers;

public static class MapperServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<RoomProfile>();
        serviceCollection.AddSingleton<PlayerProfile>();

        serviceCollection.AddSingleton(provider => new MapperConfiguration(c =>
        {
            c.AddProfile(provider.GetRequiredService<RoomProfile>());
            c.AddProfile(provider.GetRequiredService<PlayerProfile>());
        }).CreateMapper());
    }
}