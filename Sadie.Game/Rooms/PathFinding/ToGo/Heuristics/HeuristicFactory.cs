namespace Sadie.Game.Rooms.PathFinding.ToGo.Heuristics;

public static class HeuristicFactory
{
    public static ICalculateHeuristic Create(HeuristicFormula heuristicFormula)
    {
        return heuristicFormula switch
        {
            HeuristicFormula.Manhattan => new Manhattan(),
            HeuristicFormula.MaxDxdy => new MaxDxdy(),
            HeuristicFormula.DiagonalShortCut => new DiagonalShortcut(),
            HeuristicFormula.Euclidean => new Euclidean(),
            HeuristicFormula.EuclideanNoSqr => new EuclideanNoSqr(),
            _ => throw new ArgumentOutOfRangeException(nameof(heuristicFormula), heuristicFormula, null)
        };
    }
}