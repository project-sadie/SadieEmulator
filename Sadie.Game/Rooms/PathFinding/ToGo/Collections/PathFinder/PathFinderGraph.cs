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

        // Priority queue capacity = all nodes max
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

    public IEnumerable<PathFinderNode> GetSuccessors(PathFinderNode n)
    {
        foreach (var pos in _grid.GetSuccessorPositions(n.Position, _allowDiag))
        {
            yield return _grid[pos];
        }
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

        // push to heap
        var i = _count++;
        _heap[i] = n;

        while (i > 0)
        {
            var parent = (i - 1) >> 1;
            if (_heap[i].F >= _heap[parent].F) break;

            ( _heap[i], _heap[parent] ) = ( _heap[parent], _heap[i] );
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
                if (left >= _count) break;

                var right = left + 1;
                var best = left;

                if (right < _count && _heap[right].F < _heap[left].F)
                    best = right;

                if (_heap[i].F <= _heap[best].F) break;

                ( _heap[i], _heap[best] ) = ( _heap[best], _heap[i] );
                i = best;
            }
        }

        return result;
    }
}
