namespace Sadie.Game.Rooms.PathFinding.Heuristics;

public enum HeuristicFormula
{
    Manhattan = 1,
    MaxDXDY = 2,
    DiagonalShortCut = 3,
    Euclidean = 4,
    EuclideanNoSQR = 5
}