using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players;

namespace Sadie.API.Game.Rooms.Bots;

public interface IRoomBot : IRoomUnit
{ 
    PlayerBot Bot { get; }
}