using Sadie.API.Interfaces.Game.Rooms.Pathfinding.ToGo;
using Sadie.API.Interfaces.Game.Rooms.Pathfinding.ToGo.Heuristics;

namespace Sadie.Game.Rooms.PathFinding.ToGo.Heuristics;

public class EuclideanNoSqr : ICalculateHeuristic
{
    public int Calculate(IPosition source, IPosition destination)
    {
        var heuristicEstimate = 2;
        var h = (int)(heuristicEstimate * (Math.Pow(source.Row - destination.Row, 2) + Math.Pow(source.Column - destination.Column, 2)));
            
        return h;
    }
}