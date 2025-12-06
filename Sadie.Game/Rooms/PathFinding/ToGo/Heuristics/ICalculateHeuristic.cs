using Sadie.API.Interfaces.Game.Rooms.Pathfinding.ToGo;

namespace Sadie.Game.Rooms.PathFinding.ToGo.Heuristics;

public interface ICalculateHeuristic
{
    int Calculate(IPosition source, IPosition destination);
}