using Sadie.API.Interfaces.Game.Rooms.Pathfinding.ToGo;

namespace Sadie.Game.Rooms.PathFinding.ToGo.Heuristics;

public class Manhattan : ICalculateHeuristic
{
    public int Calculate(IPosition source, IPosition destination)
    {
        var heuristicEstimate = 2;
        var h = heuristicEstimate * (Math.Abs(source.Row - destination.Row) + Math.Abs(source.Column - destination.Column));
        return h;
    }
}