using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Catalog.Items;

namespace Sadie.Game.Catalog.Pages;

public class CatalogPageFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CatalogPageFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CatalogPage Create(
        int id,
        string name,
        string caption,
        string layout,
        int roleId,
        int parentId,
        int orderId,
        int icon,
        bool enabled,
        bool visible,
        string headerImage,
        string teaserImage,
        string specialImage,
        string primaryText,
        string secondaryText,
        string detailsText,
        string teaserText,
        List<CatalogItem> items)
    {
        return ActivatorUtilities.CreateInstance<CatalogPage>(
            _serviceProvider, 
            id, 
            name, 
            caption, 
            layout, 
            roleId, 
            parentId, 
            orderId, 
            icon, 
            enabled, 
            visible, 
            headerImage,
            teaserImage, 
            specialImage, 
            primaryText, 
            secondaryText, 
            detailsText, 
            teaserText,
            items);
    }
}