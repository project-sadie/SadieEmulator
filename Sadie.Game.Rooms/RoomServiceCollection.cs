using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Rooms.Categories;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Chat.Commands.General;
using Sadie.Game.Rooms.FurnitureItems;
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
        
        serviceCollection.AddTransient<IRoomUserRepository, RoomUserRepository>();
        serviceCollection.AddSingleton<IRoomUserFactory, RoomUserFactory>();
        serviceCollection.AddSingleton<IRoomFactory, RoomFactory>();
        serviceCollection.AddSingleton<IRoomRightsDao, RoomRightsDao>();
        serviceCollection.AddSingleton<IRoomDao, RoomDao>();
        serviceCollection.AddSingleton<IRoomRepository, RoomRepository>();
        
        serviceCollection.AddSingleton<RoomCategoryRepository>();
        
        serviceCollection.AddSingleton<AboutCommand>();
        
        var roomConstants = new RoomConstants();
        config.GetSection("Constants:Room").Bind(roomConstants);
        serviceCollection.AddSingleton(roomConstants);

        serviceCollection.AddSingleton<IRoomChatCommandRepository, RoomChatCommandRepository>();
        
        serviceCollection.AddSingleton<IRoomFurnitureItemRepository, RoomFurnitureItemRepository>();
    }
}