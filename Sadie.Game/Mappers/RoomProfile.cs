using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Bots;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Game.Mappers;

public class RoomProfile : Profile
{
    public RoomProfile(IServiceProvider provider)
    {
        CreateMap<RoomDto, IRoomLogic>()
            .ConstructUsing(x => new RoomLogic(
                x,
                new RoomTileMap(x.Layout.Heightmap, x.FurnitureItems),
                provider.GetRequiredService<IRoomUserRepository>(),
                provider.GetRequiredService<IRoomBotRepository>())
            {
                Name = x.Name,
                Description = x.Description
            });
    }
}