using Sadie.Game.Players.Room;

namespace Sadie.Game.Players;

public class PlayerState : IPlayerState
{
    public DateTime LastPlayerSearch { get; set; }
    public DateTime LastDirectMessage { get; set; }
    public List<PlayerRoomVisit> RoomVisits { get; }
    public DateTime LastCatalogPurchase { get; set; }

    public PlayerState()
    {
        RoomVisits = new List<PlayerRoomVisit>();
    }
}