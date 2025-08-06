using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Pathfinding;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
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
    public static void AddServices(IServiceCollection serviceCollection)
    {
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