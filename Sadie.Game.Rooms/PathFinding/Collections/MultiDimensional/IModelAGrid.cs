namespace Sadie.Game.Rooms.PathFinding.Collections.MultiDimensional;

public interface IModelAGrid<T>
{
    int Height { get; }
    int Width { get; }
    T this[int row, int column] { get; set; }
    T this[Position position] { get; set; }
    IEnumerable<Position> GetSuccessorPositions(Position node, bool optionsUseDiagonals = false);
}