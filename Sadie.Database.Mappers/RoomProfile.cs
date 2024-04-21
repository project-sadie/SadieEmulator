using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Tiles;
using Sadie.Game.Rooms.Users;

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
                new RoomTileMap(x.Layout.HeightMap, RoomHelpers.GenerateMapForRoom(x.Layout.MapSizeX, x.Layout.MapSizeY, x.Layout.HeightMap, x.FurnitureItems)),
                x.Owner,
                x.Description,
                x.MaxUsersAllowed,
                x.IsMuted,
                provider.GetRequiredService<IRoomUserRepository>(),
                x.FurnitureItems,
                x.Settings,
                x.ChatMessages,
                x.PlayerRights,
                x.PaintSettings,
                x.Tags,
                x.PlayerLikes));
    }
}