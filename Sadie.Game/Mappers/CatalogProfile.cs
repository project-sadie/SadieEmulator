using AutoMapper;
using Sadie.API.DTOs.Catalog.Pages;
using Sadie.Db.Models.Catalog.Pages;

namespace Sadie.Game.Mappers;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        CreateMap<CatalogPage, CatalogPageDto>();
    }
}