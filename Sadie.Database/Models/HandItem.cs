using System.ComponentModel.DataAnnotations;
using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models;

public class HandItem
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<FurnitureItem> FurnitureItems { get; init; } = [];
}