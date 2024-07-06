using System.ComponentModel.DataAnnotations;
using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models;

public class HandItem
{
    [Key] public int Id { get; init; }
    public string Name { get; init; }
    public ICollection<FurnitureItem> FurnitureItems { get; init; } = [];
}