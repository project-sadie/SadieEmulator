namespace Sadie.Game.Rooms.PathFinding.ToGo.Heuristics;

public class Euclidean : ICalculateHeuristic
{
    public int Calculate(Position source, Position destination)
    {
        var heuristicEstimate = 2;
        
        var calc = heuristicEstimate * Math.Sqrt(Math.Pow(source.Row - destination.Row,
            2) + Math.Pow(source.Column - destination.Column,
            2));
        
        var h = (int) calc;
            
        return h;
    }
}