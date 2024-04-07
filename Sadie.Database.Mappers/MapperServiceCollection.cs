using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Database.Mappers;

public static class MapperServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton<RoomProfile>();
        
        serviceCollection.AddSingleton(provider => new MapperConfiguration(c =>
        {
            c.AddProfile(provider.GetRequiredService<RoomProfile>());
        }).CreateMapper());
    }
}