using Sadie.API.Interfaces.Game.Rooms.Pathfinding.ToGo;

namespace Sadie.Game.Rooms.PathFinding.ToGo;

public readonly struct Position(int row = 0, int column = 0) : IPosition, IEquatable<Position>
{
    public int Row { get; } = row;
    public int Column { get; } = column;

    public bool Equals(Position other)
    {
        return Row == other.Row && Column == other.Column;
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Row * 397) ^ Column;
        }
    }

    public static bool operator ==(Position left, Position right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Position left, Position right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"[{Row},{Column}]";
    }
}