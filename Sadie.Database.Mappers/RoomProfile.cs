using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Database.Mappers;

public class RoomProfile : Profile
{
    public RoomProfile(IServiceProvider provider)
    {
        CreateMap<Room, RoomLogic>()
            .ConstructUsing(x => new RoomLogic(
                x.Id,
                x.Name,
                x.Layout,
                new RoomTileMap(x.Layout.HeightMap, x.FurnitureItems),
                x.Owner,
                x.Description,
                x.MaxUsersAllowed,
                x.IsMuted,
                provider.GetRequiredService<IRoomUserRepository>(),
                x.FurnitureItems,
                x.Settings,
                x.ChatSettings,
                x.ChatMessages,
                x.PlayerRights,
                x.PaintSettings,
                x.Tags,
                x.PlayerLikes));
    }
}