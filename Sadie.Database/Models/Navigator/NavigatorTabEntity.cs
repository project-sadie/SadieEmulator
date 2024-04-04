using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Navigator;

public class NavigatorTabEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public List<NavigatorCategoryEntity> Categories { get; set; }
}