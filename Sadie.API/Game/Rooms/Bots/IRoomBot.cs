using System.Drawing;
using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.API.Game.Rooms.Bots;

public interface IRoomBot : IRoomUnitMovementData
{ 
    int Id { get; init; }
    PlayerBot Bot { get; init; }
    Dictionary<string, string> StatusMap { get; }
    Task RunPeriodicCheckAsync();
}