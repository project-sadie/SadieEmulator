using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Catalog.Pages;

namespace Sadie.Game.Catalog.FrontPage;

public class CatalogFrontPageItemFactory(IServiceProvider serviceProvider)
{
    public CatalogFrontPageItem Create(string title, string image, CatalogPage? page)
    {
        return ActivatorUtilities.CreateInstance<CatalogFrontPageItem>(serviceProvider, title, image, page);
    }
}