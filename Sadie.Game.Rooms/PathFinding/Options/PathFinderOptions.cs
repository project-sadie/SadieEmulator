using Sadie.Game.Rooms.PathFinding.Heuristics;

namespace Sadie.Game.Rooms.PathFinding.Options;

public class PathFinderOptions
{
    public HeuristicFormula HeuristicFormula { get; }

    public bool UseDiagonals { get; set; }

    public bool PunishChangeDirection { get; set; }

    public int SearchLimit { get; }

    public PathFinderOptions()
    {
        HeuristicFormula = HeuristicFormula.Manhattan;
        UseDiagonals = true;
        SearchLimit = 2000;
    }
}