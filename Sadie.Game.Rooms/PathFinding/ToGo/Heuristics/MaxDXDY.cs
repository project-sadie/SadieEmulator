namespace Sadie.Game.Rooms.PathFinding.ToGo.Heuristics;

public class MaxDxdy : ICalculateHeuristic
{
    public int Calculate(Position source, Position destination)
    {
        var heuristicEstimate = 2;
        var h = heuristicEstimate * Math.Max(Math.Abs(source.Row - destination.Row), Math.Abs(source.Column - destination.Column));
        return h;
    }
}