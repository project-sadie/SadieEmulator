using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog;

namespace Sadie.Game.Catalog.Club;

public class CatalogClubOfferRepository(SadieContext dbContext)
{
    private List<CatalogClubOffer> _offers { get; set; } = [];

    public async Task LoadInitialDataAsync()
    {
        _offers = await dbContext
            .Set<CatalogClubOffer>()
            .ToListAsync();
    }

    public IReadOnlyCollection<CatalogClubOffer> Offers => _offers;
}