using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models.Players;

public class PlayerFurnitureItem
{
    public int Id { get; init; }
    public int PlayerId { get; init; }
    public FurnitureItem FurnitureItem { get; init; }
    public string LimitedData { get; init; }
    public string MetaData { get; init; }
    public DateTime CreatedAt { get; init; }
}