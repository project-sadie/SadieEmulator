using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Catalog.Pages;

namespace Sadie.Game.Catalog.FrontPage;

public class CatalogFrontPageItemFactory(IServiceProvider serviceProvider)
{
    public CatalogFrontPageItem Create(int id, string title, string image, CatalogFrontPageItemType type, string productName, CatalogPage? page)
    {
        return ActivatorUtilities.CreateInstance<CatalogFrontPageItem>(serviceProvider, id, title, image, type, productName, page);
    }
}