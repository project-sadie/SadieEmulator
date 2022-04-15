using Sadie.Game.Players.Room;

namespace Sadie.Game.Players;

public class PlayerState : IPlayerState
{
    public DateTime LastSearch { get; set; }
    public DateTime LastMessage { get; set; }
    public List<PlayerRoomVisit> RoomVisits { get; }

    public PlayerState()
    {
        RoomVisits = new List<PlayerRoomVisit>();
    }
}