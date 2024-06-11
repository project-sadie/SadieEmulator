using System.Drawing;
using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.API.Game.Rooms.Bots;

public interface IRoomBot
{ 
    int Id { get; init; }
    PlayerBot Bot { get; init; }
    Point Point { get; protected set;  }
    double PointZ { get; protected set;  }
    HDirection DirectionHead { get; set; }
    HDirection Direction { get; set; }
    Dictionary<string, string> StatusMap { get; }
    Task RunPeriodicCheckAsync();
}