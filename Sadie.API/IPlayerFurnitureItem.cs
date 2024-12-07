using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API;

public interface IPlayerFurnitureItem
{
    int Id { get; init; }
    int PlayerId { get; set; }
    Player Player { get; set; }
    FurnitureItem FurnitureItem { get; init; }
    PlayerFurnitureItemPlacementData? PlacementData { get; set; }
    string LimitedData { get; init; }
    string MetaData { get; set; }
    DateTime CreatedAt { get; init; }
}