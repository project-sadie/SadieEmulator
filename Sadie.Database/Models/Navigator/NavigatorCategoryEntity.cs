namespace Sadie.Database.Models.Navigator;

public class NavigatorCategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CodeName { get; set; }
    public int OrderId { get; set; }
    public int TabId { get; set; }
    public NavigatorTabEntity Tab { get; set; }
}