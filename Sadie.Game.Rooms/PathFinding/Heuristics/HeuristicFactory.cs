namespace Sadie.Game.Rooms.PathFinding.Heuristics
{
    public static class HeuristicFactory
    {
        public static ICalculateHeuristic Create(HeuristicFormula heuristicFormula)
        {
            return heuristicFormula switch
            {
                HeuristicFormula.Manhattan => new Manhattan(),
                HeuristicFormula.MaxDXDY => new MaxDXDY(),
                HeuristicFormula.DiagonalShortCut => new DiagonalShortcut(),
                HeuristicFormula.Euclidean => new Euclidean(),
                HeuristicFormula.EuclideanNoSQR => new EuclideanNoSQR(),
                _ => throw new ArgumentOutOfRangeException(nameof(heuristicFormula), heuristicFormula, null)
            };
        }
    }
}