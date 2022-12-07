namespace Sadie.Game.Catalog;

public class CatalogPageFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CatalogPageFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CatalogPage Create(int id, string name, string caption, string layout, int roleId, int parentId, int orderId, int icon, bool enabled, bool visible, List<CatalogPage> childPages, List<CatalogItem> items)
    {
        return new CatalogPage(
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
            childPages,
            items);
    }
}