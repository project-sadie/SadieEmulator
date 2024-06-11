using Sadie.Database.Models.Players;

namespace Sadie.API.Game.Rooms.Bots;

public interface IRoomBot : IRoomUnitMovementData
{ 
    int Id { get; init; }
    PlayerBot Bot { get; init; }
    Dictionary<string, string> StatusMap { get; }
    Task RunPeriodicCheckAsync();
}