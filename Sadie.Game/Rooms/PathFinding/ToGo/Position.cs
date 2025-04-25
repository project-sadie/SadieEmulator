namespace Sadie.Game.Rooms.PathFinding.ToGo;

/// <summary>
/// A point in a matrix. P(row, column)
/// </summary>
public readonly struct Position(int row = 0, int column = 0)
{
    /// <summary>
    /// The row in the matrix
    /// </summary>
    public int Row { get; } = row;

    /// <summary>
    /// The column in the matrix
    /// </summary>
    public int Column { get; } = column;

    public static bool operator ==(Position a, Position b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Position a, Position b)
    {
        return !a.Equals(b);
    }

    public override bool Equals(Object other)
    {
        if (other is Position otherPoint)
        {
            return Row == otherPoint.Row && Column == otherPoint.Column;
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + Row.GetHashCode();
            hash = hash * 23 + Column.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return $"[{Row}.{Column}]";
    }
}