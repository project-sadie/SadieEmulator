using Sadie.Game.Navigator.Categories;

namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTab(
    int id,
    string name,
    List<NavigatorCategory> categories)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public List<NavigatorCategory> Categories { get; } = categories;
}