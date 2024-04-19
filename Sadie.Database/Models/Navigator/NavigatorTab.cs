using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Navigator;

public class NavigatorTab
{
    [Key]
    public int Id { get; init; }
    public string Name { get; init; }
    public ICollection<NavigatorCategory> Categories { get; init; } = [];
}