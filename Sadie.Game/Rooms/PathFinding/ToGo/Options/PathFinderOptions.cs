using Sadie.Game.Rooms.PathFinding.ToGo.Heuristics;

namespace Sadie.Game.Rooms.PathFinding.ToGo.Options;

public class PathFinderOptions
{
    public HeuristicFormula HeuristicFormula { get; } = HeuristicFormula.Manhattan;

    public bool UseDiagonals { get; init; } = true;

    public bool PunishChangeDirection { get; set; }

    public int SearchLimit { get; } = 2000;
}