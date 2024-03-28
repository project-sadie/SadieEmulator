using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Rooms.Categories;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Chat.Commands.General;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddTransient<IRoomUserRepository, RoomUserRepository>();
        serviceCollection.AddSingleton<IRoomUserFactory, RoomUserFactory>();
        serviceCollection.AddSingleton<IRoomFactory, RoomFactory>();
        serviceCollection.AddSingleton<IRoomRightsDao, RoomRightsDao>();
        serviceCollection.AddSingleton<IRoomDao, RoomDao>();
        serviceCollection.AddSingleton<IRoomRepository, RoomRepository>();
            
        serviceCollection.AddSingleton<RoomCategoryFactory>();
        serviceCollection.AddSingleton<IRoomCategoryDao, RoomCategoryDao>();
        serviceCollection.AddSingleton<IRoomCategoryRepository, RoomCategoryRepository>();
        
        serviceCollection.AddSingleton<AboutCommand>();
        
        var roomConstants = new RoomConstants();
        config.GetSection("Constants:Room").Bind(roomConstants);
        serviceCollection.AddSingleton(roomConstants);

        serviceCollection.AddSingleton(provider => new ConcurrentDictionary<string, IRoomChatCommand>
        {
            ["about"] = provider.GetRequiredService<AboutCommand>()
        });
        
        serviceCollection.AddSingleton<IRoomChatCommandRepository, RoomChatCommandRepository>();
        
        serviceCollection.AddSingleton<IRoomFurnitureItemRepository, RoomFurnitureItemRepository>();
        serviceCollection.AddSingleton<IRoomFurnitureItemDao, RoomFurnitureItemDao>();
    }
}