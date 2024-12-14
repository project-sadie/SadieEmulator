using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Furniture.Processors;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Pathfinding;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Game.Rooms.Bots;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.PathFinding;
using Sadie.Game.Rooms.Services;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public static class RoomServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<AbstractRoomChatCommand>())
            .As<IRoomChatCommand>()
            .WithSingletonLifetime());

        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<AbstractRoomFurnitureItemInteractor>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<IRoomFurnitureItemProcessor>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        if (config.GetValue("UseInMemoryRooms", false))
        {
            serviceCollection.AddSingleton<ConcurrentDictionary<int, IRoomLogic>>(provider =>
            {
                var mapper = provider.GetRequiredService<IMapper>();
                var dbContext = provider.GetRequiredService<SadieContext>();
                var rooms = dbContext.Rooms.ToList();

                var roomsDictionary = rooms
                    .Select(x => mapper.Map<IRoomLogic>(x))
                    .ToDictionary(x => x.Id, x => x);
                
                return new ConcurrentDictionary<int, IRoomLogic>(roomsDictionary);
            });
        }
        else
        {
            serviceCollection.AddSingleton<ConcurrentDictionary<int, IRoomLogic>>([]);
        }
        
        serviceCollection.AddTransient<IRoomUserRepository, RoomUserRepository>();
        serviceCollection.AddTransient<IRoomBotRepository, RoomBotRepository>();
        serviceCollection.AddSingleton<IRoomUserFactory, RoomUserFactory>();
        serviceCollection.AddSingleton<IRoomBotFactory, RoomBotFactory>();
        serviceCollection.AddSingleton<IRoomRepository, RoomRepository>();

        serviceCollection.AddSingleton<IRoomChatCommandRepository, RoomChatCommandRepository>();
        serviceCollection.AddSingleton<IRoomFurnitureItemInteractorRepository, RoomFurnitureItemInteractorRepository>();
        serviceCollection.AddTransient<IRoomWiredService, RoomWiredService>();
        serviceCollection.AddSingleton<IRoomTileMapHelperService, RoomTileMapHelperService>();
        serviceCollection.AddSingleton<IRoomHelperService, RoomHelperService>();
        serviceCollection.AddSingleton<IRoomFurnitureItemHelperService, RoomFurnitureItemHelperService>();
        serviceCollection.AddSingleton<IRoomPathFinderHelperService, RoomPathFinderHelperService>();
    }
}