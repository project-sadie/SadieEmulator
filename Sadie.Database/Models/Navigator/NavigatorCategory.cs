namespace Sadie.Database.Models.Navigator;

public class NavigatorCategory
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string CodeName { get; init; }
    public required int OrderId { get; init; }
    public required int TabId { get; init; }
    public NavigatorTab? Tab { get; init; }
}