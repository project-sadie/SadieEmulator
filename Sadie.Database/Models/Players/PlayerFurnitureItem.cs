using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models.Players;

public class PlayerFurnitureItem
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public FurnitureItem FurnitureItem { get; set; }
    public string LimitedData { get; set; }
    public string MetaData { get; set; }
    public DateTime CreatedAt { get; set; }
}