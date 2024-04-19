namespace Sadie.Database.Models.Navigator;

public class NavigatorCategory
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string CodeName { get; init; }
    public int OrderId { get; init; }
    public int TabId { get; init; }
    public NavigatorTab Tab { get; init; }
}