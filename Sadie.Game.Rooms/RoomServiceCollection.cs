using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Rooms.Bots;
using Sadie.Game.Rooms.Chat;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Chat.Commands.Server;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public static class RoomServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.Where(x => 
                x is { IsClass: true, IsAbstract: false } && 
                x.IsSubclassOf(typeof(AbstractRoomChatCommand))))
            .As<IRoomChatCommand>()
            .WithSingletonLifetime());

        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.Where(x => 
                x is { IsClass: true, IsAbstract: false } && 
                x.IsAssignableTo(typeof(IRoomFurnitureItemInteractor))))
            .As<IRoomFurnitureItemInteractor>()
            .WithTransientLifetime());

        serviceCollection.AddTransient<IRoomUserRepository, RoomUserRepository>();
        serviceCollection.AddTransient<IRoomBotRepository, RoomBotRepository>();
        serviceCollection.AddSingleton<RoomUserFactory>();
        serviceCollection.AddSingleton<RoomBotFactory>();
        serviceCollection.AddSingleton<RoomRepository, RoomRepository>();

        serviceCollection.AddSingleton<AboutCommand>();

        serviceCollection.AddSingleton<IRoomChatCommandRepository, RoomChatCommandRepository>();
        serviceCollection.AddSingleton<RoomFurnitureItemInteractorRepository>();
    }
}