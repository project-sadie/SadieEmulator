using Sadie.Game.Navigator.Categories;

namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTab
{
    public int Id { get; }
    public string Name { get; }
    public List<NavigatorCategory> Categories { get; }
    
    public NavigatorTab(
        int id,
        string name,
        List<NavigatorCategory> categories)
    {
        Id = id;
        Name = name;
        Categories = categories;
    }
}