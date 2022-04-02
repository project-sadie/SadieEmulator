using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Rooms.Categories;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddTransient<IRoomUserRepository, RoomUserRepository>();
        serviceCollection.AddSingleton<IRoomUserFactory, RoomUserFactory>();
        serviceCollection.AddSingleton<IRoomFactory, RoomFactory>();
        serviceCollection.AddSingleton<IRoomDao, RoomDao>();
        serviceCollection.AddSingleton<IRoomRepository, RoomRepository>();
            
        serviceCollection.AddSingleton<RoomCategoryFactory>();
        serviceCollection.AddSingleton<IRoomCategoryDao, RoomCategoryDao>();
        serviceCollection.AddSingleton<IRoomCategoryRepository, RoomCategoryRepository>();
        
        var roomConstants = new RoomConstants();
        config.GetSection("Constants:Room").Bind(roomConstants);
        serviceCollection.AddSingleton(roomConstants);
    }
}