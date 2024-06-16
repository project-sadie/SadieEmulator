using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API.Game.Players;

public interface IPlayerState
{
    DateTime LastPlayerSearch { get; set; }
    DateTime LastDirectMessage { get; set; }
    DateTime LastCatalogPurchase { get; set; }
    DateTime LastSubscriptionModification { get; set; }
    string CatalogMode { get; set; }
    PlayerFurnitureItemPlacementData? Teleport { get; set; }
}