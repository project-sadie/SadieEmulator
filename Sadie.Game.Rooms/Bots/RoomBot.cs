using System.Drawing;
using Sadie.API.Game.Rooms.Bots;
using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Bots;

public class RoomBot : IRoomBot
{
    public required PlayerBot Bot { get; init; }
    public required Point Point { get; set;  }
    public required double PointZ { get; set;  }
    public HDirection DirectionHead { get; set; }
    public HDirection Direction { get; set; }
}