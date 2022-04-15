using Sadie.Game.Players.Room;

namespace Sadie.Game.Players;

public interface IPlayerState
{
    DateTime LastSearch { get; set; }
    DateTime LastMessage { get; set; }
    List<PlayerRoomVisit> RoomVisits { get; }
}