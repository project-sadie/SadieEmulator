using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Navigator.Categories;

public class NavigatorCategoryFactory(IServiceProvider serviceProvider)
{
    public NavigatorCategory Create(int id, string name, string codeName, int orderId)
    {
        return ActivatorUtilities.CreateInstance<NavigatorCategory>(
            serviceProvider,
            id, 
            name,
            codeName, 
            orderId);
    }
}