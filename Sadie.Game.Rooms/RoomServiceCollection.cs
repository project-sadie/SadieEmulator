using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;
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
        Assembly.LoadFile("/mnt/storage/dev/habbo/sadie/SadieEmulator/PluginTest/bin/Debug/net8.0/PluginTest.dll");
        // /mnt/storage/dev/habbo/sadie/SadieEmulator/PluginTest/bin/Debug/net8.0/PluginTest.dll
        
        serviceCollection.Scan(scan => scan
            .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.Where(x => x is { IsClass: true, IsAbstract: false } && x.IsSubclassOf(typeof(AbstractRoomChatCommand))))
            .As<IRoomChatCommand>()
            .WithSingletonLifetime());

        serviceCollection.Scan(scan => scan
            .FromAssemblies()
            .AddClasses(classes => classes.AssignableTo<IRoomFurnitureItemInteractor>())
            .AsSelfWithInterfaces()
            .WithSingletonLifetime());

        serviceCollection.AddTransient<IRoomUserRepository, RoomUserRepository>();
        serviceCollection.AddSingleton<RoomUserFactory>();
        serviceCollection.AddSingleton<RoomRepository, RoomRepository>();

        serviceCollection.AddSingleton<AboutCommand>();

        serviceCollection.AddSingleton<IRoomChatCommandRepository, RoomChatCommandRepository>();
        serviceCollection.AddSingleton<RoomFurnitureItemInteractorRepository>();
    }
}