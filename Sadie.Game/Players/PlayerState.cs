using Sadie.API.Game.Players;
using Sadie.Db.Models.Players.Furniture;

namespace Sadie.Game.Players;

public class PlayerState : IPlayerState
{
    public DateTime LastPlayerSearch { get; set; }
    public DateTime LastDirectMessage { get; set; }
    public DateTime LastCatalogPurchase { get; set; }
    public DateTime LastSubscriptionModification { get; set; }
    public string CatalogMode { get; set; }
    public PlayerFurnitureItemPlacementData? Teleport { get; set; }
    public int CurrentRoomId { get; set; }
}