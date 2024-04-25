namespace Sadie.Game.Rooms.PathFinding.Heuristics;

public interface ICalculateHeuristic
{
    int Calculate(Position source, Position destination);
}