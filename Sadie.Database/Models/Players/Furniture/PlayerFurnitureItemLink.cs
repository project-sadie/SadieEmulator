namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItemLink
{
    public int Id { get; init; }
    public required int ParentId { get; init; }
    public PlayerFurnitureItem? Parent { get; init; }
    public required int ChildId { get; init; }
    public PlayerFurnitureItem? Child { get; init; }
}