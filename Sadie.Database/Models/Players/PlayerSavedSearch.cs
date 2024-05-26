namespace Sadie.Database.Models.Players;

public class PlayerSavedSearch
{
    public int Id { get; init; }
    public string? Search { get; init; }
    public string? Filter { get; init; }
    public int PlayerId { get; init; }
}