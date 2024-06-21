using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Players;

public class PlayerTag
{
    [Key] public int Id { get; init; }
    public int PlayerId { get; init; }
    public string? Name { get; init; }
}