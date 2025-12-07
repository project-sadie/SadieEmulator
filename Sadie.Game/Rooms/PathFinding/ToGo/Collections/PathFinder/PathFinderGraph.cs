using Sadie.API.Interfaces.Game.Rooms.Pathfinding.ToGo;
using Sadie.Game.Rooms.PathFinding.ToGo.Collections.MultiDimensional;

namespace Sadie.Game.Rooms.PathFinding.ToGo.Collections.PathFinder;

internal class PathFinderGraph : IModelAGraph<PathFinderNode>
{
    private readonly bool _allowDiag;
    private readonly Grid<PathFinderNode> _grid;
    private readonly bool[,] _visited;
    private readonly PathFinderNode[] _heap;
    private int _count;

    public bool HasOpenNodes => _count > 0;

    public PathFinderGraph(int height, int width, bool allowDiag)
    {
        _allowDiag = allowDiag;
        _grid = new Grid<PathFinderNode>(height, width);
        _visited = new bool[height, width];
        _heap = new PathFinderNode[height * width];
        Reset();
    }

    public void Reset()
    {
        for (var r = 0; r < _grid.Height; r++)
        {
            for (var c = 0; c < _grid.Width; c++)
            {
                var pos = new Position(r, c);
                _grid[r, c] = new PathFinderNode(pos, 0, 0, pos);
                _visited[r, c] = false;
            }
        }
        _count = 0;
    }

    private readonly struct SuccessorEnumerable(Grid<PathFinderNode> g, IPosition p, bool diag)
        : IEnumerable<PathFinderNode>
    {
        IEnumerator<PathFinderNode> IEnumerable<PathFinderNode>.GetEnumerator()
        {
            return new SuccessorEnumerator(g, p, diag);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SuccessorEnumerator(g, p, diag);
        }
    }

    private struct SuccessorEnumerator(Grid<PathFinderNode> g, IPosition p, bool diag) : IEnumerator<PathFinderNode>
    {
        private int _i = -1;
        private PathFinderNode _current = default;

        private static readonly (int r, int c)[] Offsets =
        {
            (-1,0),(1,0),(0,-1),(0,1),
            (-1,-1),(-1,1),(1,-1),(1,1)
        };

        public bool MoveNext()
        {
            while (true)
            {
                _i++;
                if (_i >= 8)
                {
                    return false;
                }
                if (_i >= 4 && !diag)
                {
                    continue;
                }

                var (dr, dc) = Offsets[_i];
                var nr = p.Row + dr;
                var nc = p.Column + dc;

                if ((uint)nr >= (uint)g.Height || (uint)nc >= (uint)g.Width)
                {
                    continue;
                }

                _current = g[nr, nc];
                return true;
            }
        }

        public PathFinderNode Current => _current;
        object System.Collections.IEnumerator.Current => _current;
        public void Reset() { _i = -1; }
        public void Dispose() { }
    }

    public IEnumerable<PathFinderNode> GetSuccessors(PathFinderNode n)
    {
        return new SuccessorEnumerable(_grid, n.Position, _allowDiag);
    }

    public PathFinderNode GetParent(PathFinderNode n)
    {
        return _grid[n.ParentNodePosition];
    }

    public bool WasVisited(IPosition pos)
    {
        return _visited[pos.Row, pos.Column];
    }

    public void OpenNode(PathFinderNode n)
    {
        _visited[n.Position.Row, n.Position.Column] = true;
        _grid[n.Position] = n;

        var i = _count++;
        _heap[i] = n;

        while (i > 0)
        {
            var parent = (i - 1) >> 1;
            if (_heap[i].F >= _heap[parent].F)
            {
                break;
            }

            (_heap[i], _heap[parent]) = (_heap[parent], _heap[i]);
            i = parent;
        }
    }

    public PathFinderNode GetOpenNodeWithSmallestF()
    {
        var result = _heap[0];
        _count--;

        if (_count > 0)
        {
            _heap[0] = _heap[_count];
            var i = 0;

            while (true)
            {
                var left = (i << 1) + 1;
                if (left >= _count)
                {
                    break;
                }

                var right = left + 1;
                var best = left;

                if (right < _count && _heap[right].F < _heap[left].F)
                {
                    best = right;
                }

                if (_heap[i].F <= _heap[best].F)
                {
                    break;
                }

                (_heap[i], _heap[best]) = (_heap[best], _heap[i]);
                i = best;
            }
        }

        return result;
    }
}
