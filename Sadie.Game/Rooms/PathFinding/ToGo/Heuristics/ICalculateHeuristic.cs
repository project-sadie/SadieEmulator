namespace Sadie.Game.Rooms.PathFinding.ToGo.Heuristics;

public interface ICalculateHeuristic
{
    int Calculate(Position source, Position destination);
}