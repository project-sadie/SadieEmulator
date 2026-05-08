using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.DTOs.Rooms;
using Sadie.API.DTOs.Rooms.Chat;
using Sadie.API.DTOs.Rooms.Rights;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Bots;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Db.Models.Rooms;
using Sadie.Db.Models.Rooms.Chat;
using Sadie.Db.Models.Rooms.Rights;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.PathFinding;
using Sadie.Game.Rooms.PathFinding.ToGo;
using Sadie.Game.Rooms.PathFinding.ToGo.Options;

namespace Sadie.Game.Mappers;

public class RoomProfile : Profile
{
    public RoomProfile(IServiceProvider provider)
    {
        CreateMap<RoomDto, IRoomLogic>()
            .ConstructUsing((x, _) =>
            {
                var tileMap = new RoomTileMap(x.Layout!.Heightmap ?? "", x.FurnitureItems);
                var worldArray = tileMap.GetWorldArrayFromTileMap(tileMap, default, []);
                var worldGrid = new WorldGrid(worldArray);
                var pathFinder = new RoomPathFinder(worldGrid.Height, worldGrid.Width, new PathFinderOptions
                {
                    UseDiagonals = x.Settings?.WalkDiagonal ?? true
                });

                return new RoomLogic(
                        x,
                        tileMap,
                        pathFinder,
                        provider.GetRequiredService<IRoomUserRepository>(),
                        provider.GetRequiredService<IRoomBotRepository>())
                    {
                        Name = x.Name,
                        Description = x.Description
                    };
            });

        CreateMap<RoomLayout, RoomLayoutDto>().ReverseMap();
        CreateMap<Room, RoomDto>().ReverseMap();
        CreateMap<RoomChatSettings, RoomChatSettingsDto>().ReverseMap();
        CreateMap<RoomChatMessage, RoomChatMessageDto>().ReverseMap();
        CreateMap<List<RoomChatMessage>, List<RoomChatMessageDto>>().ReverseMap();
        CreateMap<RoomPaintSettings, RoomPaintSettingsDto>().ReverseMap();
        CreateMap<RoomSettings, RoomSettingsDto>().ReverseMap();
        CreateMap<RoomPlayerRight,  RoomPlayerRightDto>();
        CreateMap<RoomTag, RoomTagDto>();
        CreateMap<RoomDimmerSettings, RoomDimmerSettingsDto>();
    }
}