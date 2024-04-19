using Sadie.Game.Players.RoomVisits;

namespace Sadie.Game.Players;

public interface IPlayerState
{
    DateTime LastPlayerSearch { get; set; }
    DateTime LastDirectMessage { get; set; }
    List<PlayerRoomVisit> RoomVisits { get; }
    DateTime LastCatalogPurchase { get; set; }
    DateTime LastSubscriptionModification { get; set; }
}