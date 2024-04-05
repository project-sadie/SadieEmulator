using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Navigator;

public class NavigatorTabDto
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public List<NavigatorCategoryDto> Categories { get; set; }
}