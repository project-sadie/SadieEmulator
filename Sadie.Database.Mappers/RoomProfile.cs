using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models;
using Sadie.Game.Rooms;
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
                new RoomLayoutData(x.Layout.HeightMap, RoomHelpers.BuildTileListFromHeightMap(x.Layout.HeightMap, x.FurnitureItems)),
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