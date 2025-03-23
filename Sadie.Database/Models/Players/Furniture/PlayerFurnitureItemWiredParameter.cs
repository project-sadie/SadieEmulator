using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItemWiredParameter
{
    [Key] public int Id { get; init; }
    public required int Value { get; init; }
    public int PlayerFurnitureItemWiredDataId { get; init; }
}