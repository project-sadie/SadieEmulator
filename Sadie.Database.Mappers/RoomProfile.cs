using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db.Models.Rooms;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Database.Mappers;

public class RoomProfile : Profile
{
    public RoomProfile(IServiceProvider provider)
    {
        CreateMap<Room, IRoomLogic>()
            .ConstructUsing(x => new RoomLogic(
                x.Id,
                x.Name,
                x.Layout,
                new RoomTileMap(x.Layout.Heightmap, x.FurnitureItems),
                x.Owner,
                x.Description,
                x.MaxUsersAllowed,
                x.IsMuted,
                provider.GetRequiredService<IRoomUserRepository>(),
                provider.GetRequiredService<IRoomBotRepository>(),
                x.FurnitureItems,
                x.Settings,
                x.ChatSettings,
                x.ChatMessages,
                x.PlayerRights,
                x.PaintSettings,
                x.Tags,
                x.PlayerLikes)
            {
                Name = x.Name,
                Description = x.Description
            });
    }
}