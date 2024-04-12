using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Chat.Commands.General;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public static class RoomServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblyOf<IRoomChatCommand>()
            .AddClasses(classes => classes.AssignableTo<IRoomChatCommand>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
        
        serviceCollection.Scan(scan => scan
            .FromAssemblyOf<IRoomFurnitureItemInteractor>()
            .AddClasses(classes => classes.AssignableTo<IRoomFurnitureItemInteractor>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
        
        serviceCollection.AddTransient<IRoomUserRepository, RoomUserRepository>();
        serviceCollection.AddSingleton<RoomUserFactory>();
        serviceCollection.AddSingleton<RoomRepository, RoomRepository>();
        
        serviceCollection.AddSingleton<AboutCommand>();

        serviceCollection.AddSingleton<IRoomChatCommandRepository, RoomChatCommandRepository>();
        serviceCollection.AddSingleton<RoomFurnitureItemInteractorRepository>();
    }
}