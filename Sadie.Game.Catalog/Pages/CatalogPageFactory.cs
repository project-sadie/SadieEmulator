using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Catalog;

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
        List<CatalogPage> pages,
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
            pages,
            items);
    }
}