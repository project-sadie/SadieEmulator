using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Navigator;

public class NavigatorTab
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<NavigatorCategory> Categories { get; set; } = [];
}