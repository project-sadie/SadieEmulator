using Sadie.API;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.Game.Players;

public class PlayerState : IPlayerState
{
    public DateTime LastPlayerSearch { get; set; }
    public DateTime LastDirectMessage { get; set; }
    public DateTime LastCatalogPurchase { get; set; }
    public DateTime LastSubscriptionModification { get; set; }
    public string CatalogMode { get; set; }
    public RoomFurnitureItem? Teleport { get; set; }
}