using AutoMapper;
using Sadie.API.DTOs.Furniture;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.Db.Models.Furniture;
using Sadie.Db.Models.Players.Furniture;

namespace Sadie.Game.Mappers;

public class FurnitureItemProfile : Profile
{
    public FurnitureItemProfile()
    {
        CreateMap<FurnitureItem, FurnitureItemDto>().ReverseMap();
        
        CreateMap<PlayerFurnitureItem, PlayerFurnitureItemDto>()
            .ForMember(dest => dest.FurnitureItem, opt => opt.MapFrom(src => src.FurnitureItem));

        CreateMap<PlayerFurnitureItemDto, PlayerFurnitureItem>()
            .ForMember(dest => dest.FurnitureItem, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureItemId, opt => opt.MapFrom(src => src.FurnitureItem.Id));
    }
}