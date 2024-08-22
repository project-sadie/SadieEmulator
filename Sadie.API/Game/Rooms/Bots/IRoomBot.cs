using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players;

namespace Sadie.API.Game.Rooms.Bots;

public interface IRoomBot : IRoomUnitData
{ 
    int Id { get; }
    PlayerBot Bot { get; }
    Dictionary<string, string> StatusMap { get; }
    Task RunPeriodicCheckAsync();
}