using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Navigator.Categories;

public class NavigatorCategoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NavigatorCategoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public NavigatorCategory Create(int id, string name, string codeName, int orderId)
    {
        return ActivatorUtilities.CreateInstance<NavigatorCategory>(
            _serviceProvider,
            id, 
            name,
            codeName, 
            orderId);
    }
}