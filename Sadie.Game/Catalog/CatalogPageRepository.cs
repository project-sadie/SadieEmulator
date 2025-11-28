using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Catalog.Pages;
using Sadie.API.Interfaces.Game.Catalog;
using Sadie.Db;

namespace Sadie.Game.Catalog;

public class CatalogPagePageRepository(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper) : ICatalogPageRepository
{
    public IReadOnlyList<CatalogPageDto> Pages { get; private set; } = [];
    
    public async Task LoadAsync()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var efPages = context.CatalogPages
            .Include(p => p.Items)
            .ThenInclude(i => i.FurnitureItems)
            .Include(i => i.Pages)
            .ThenInclude(i => i.Pages)
            .ThenInclude(i => i.Pages)
            .ThenInclude(i => i.Pages)
            .ToList();
        
        Pages = mapper.Map<IReadOnlyList<CatalogPageDto>>(efPages);
    }
}