using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.API;

public interface IPlayerState
{
    DateTime LastPlayerSearch { get; set; }
    DateTime LastDirectMessage { get; set; }
    DateTime LastCatalogPurchase { get; set; }
    DateTime LastSubscriptionModification { get; set; }
    string CatalogMode { get; set; }
    RoomFurnitureItem? Teleport { get; set; }
}