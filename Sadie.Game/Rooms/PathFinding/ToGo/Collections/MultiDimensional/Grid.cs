namespace Sadie.Game.Rooms.PathFinding.ToGo.Collections.MultiDimensional;

public class Grid<T> : IModelAGrid<T>
{
    private readonly T[] _grid;
    public Grid(int height, int width)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);

        Height = height;
        Width = width;

        _grid = new T[height * width];
    }

    public int Height { get; }

    public int Width { get; }
        
    public IEnumerable<Position> GetSuccessorPositions(Position node, bool optionsUseDiagonals = false)
    {
        var offsets = GridOffsets.GetOffsets(optionsUseDiagonals);
        foreach (var neighbourOffset in offsets)
        {
            var successorRow = node.Row + neighbourOffset.row;
                
            if (successorRow < 0 || successorRow >= Height)
            {
                continue;
                    
            }
                
            var successorColumn = node.Column + neighbourOffset.column;

            if (successorColumn < 0 || successorColumn >= Width)
            {
                continue;
            }
                
            yield return new Position(successorRow, successorColumn);
        }
    }

    public T this[Position position]
    {
        get => _grid[ConvertRowColumnToIndex(position.Row, position.Column)];
        set => _grid[ConvertRowColumnToIndex(position.Row, position.Column)] = value;
    }
    
    public T this[int row, int column]
    {
        get => _grid[ConvertRowColumnToIndex(row, column)];
        set => _grid[ConvertRowColumnToIndex(row, column)] = value;
    }

    private int ConvertRowColumnToIndex(int row, int column)
    {
        return Width * row + column;
    }
}