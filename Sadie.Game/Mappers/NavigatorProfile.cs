using AutoMapper;
using Sadie.API.DTOs.Navigator;
using Sadie.Db.Models.Navigator;

namespace Sadie.Game.Mappers;

public class NavigatorProfile : Profile
{
    public NavigatorProfile()
    {
        CreateMap<NavigatorCategory, NavigatorCategoryDto>();
        CreateMap<NavigatorTab, NavigatorTabDto>();
    }
}