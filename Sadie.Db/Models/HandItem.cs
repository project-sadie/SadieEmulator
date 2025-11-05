using System.ComponentModel.DataAnnotations;
using Sadie.Db.Models.Furniture;

namespace Sadie.Db.Models;

public class HandItem
{
    [Key] public int Id { get; init; }
    public string Name { get; init; }
    public ICollection<FurnitureItem> FurnitureItems { get; init; } = [];
}